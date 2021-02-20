// Licensed under the MIT License. See LICENSE file for full license information.

using System;
using System.IO.Pipelines;
using System.Threading;
using System.Threading.Tasks;

namespace Motormoth.Extensions.IO.Pipelines
{
    /// <summary>
    /// Provides a set of extension methods for copying data from pipe reader to pipe writer.
    /// </summary>
    public static partial class PipeReaderCopyDataExtensions
    {
        /// <summary>
        /// Asynchronously reads a sequence of bytes from a reader and writes them to a writer.
        /// </summary>
        /// <param name="source">The reader from which data will be copied.</param>
        /// <param name="destination">The writer to which data will be copied.</param>
        /// <param name="copyLimit">
        /// The maximum number of bytes to copy. The default value is -1. If the value is negative,
        /// all remaining data in <paramref name="source"/> will be copied.
        /// </param>
        /// <param name="cancellationToken">
        /// The token to monitor for cancellation requests. The default value is <see cref="CancellationToken.None"/>.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous copy operation. The result value contains the
        /// total number of bytes copied into <paramref name="destination"/>.
        /// </returns>
        public static async ValueTask<long> CopyDataAsync(this PipeReader source, PipeWriter destination,
            long copyLimit = -1, CancellationToken cancellationToken = default)
        {
            CheckCopyDataArguments(source, destination);
            var copied = 0L;
            var copiedAll = copyLimit == 0;
            while (!copiedAll)
            {
                var read = await source.ReadAsync(cancellationToken)
                    .ConfigureAwait(continueOnCapturedContext: false);
                if (read.IsCanceled)
                {
                    throw new OperationCanceledException(Error_ReadCanceledOnReader);
                }

                var readBuffer = read.Buffer;
                if (readBuffer.IsEmpty)
                {
                    break;
                }

                var examined = readBuffer.End;
                var consumed = readBuffer.Start;
                try
                {
                    foreach (var readSegment in readBuffer)
                    {
                        var writeBuffer = GetWriteBuffer(readSegment, copyLimit, copied, ref copiedAll);
                        if (writeBuffer.IsEmpty)
                        {
                            break;
                        }

                        var flush = await destination.WriteAsync(writeBuffer, cancellationToken)
                            .ConfigureAwait(continueOnCapturedContext: false);
                        if (flush.IsCanceled)
                        {
                            throw new OperationCanceledException(Error_FlushCanceledOnWriter);
                        }

                        copied += writeBuffer.Length;
                        consumed = readBuffer.GetPosition(writeBuffer.Length, consumed);
                    }
                }
                finally
                {
                    source.AdvanceTo(consumed, examined);
                }

                if (read.IsCompleted)
                {
                    break;
                }
            }

            return copied;
        }

        private static void CheckCopyDataArguments(PipeReader source, PipeWriter destination)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (destination is null)
            {
                throw new ArgumentNullException(nameof(destination));
            }
        }

        private static ReadOnlyMemory<byte> GetWriteBuffer(ReadOnlyMemory<byte> readBuffer,
            long copyLimit, long copied, ref bool copiedAll)
        {
            if (copyLimit >= 0)
            {
                var left = copyLimit - copied;
                if (left < readBuffer.Length)
                {
                    copiedAll = true;
                    if (left == 0)
                    {
                        return ReadOnlyMemory<byte>.Empty;
                    }
                    else
                    {
                        return readBuffer.Slice(0, (int)left);
                    }
                }
            }

            return readBuffer;
        }
    }
}