// Licensed under the MIT License. See LICENSE file for full license information.

using System;
using System.Buffers;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Motormoth.Extensions.IO
{
    /// <summary>
    /// Provides a set of extension methods to copy data between streams.
    /// </summary>
    public static partial class StreamCopyDataExtensions
    {
        /// <summary>
        /// Reads a sequence of bytes from a source stream and writes them to a destination stream.
        /// </summary>
        /// <param name="source">The stream from which data will be copied.</param>
        /// <param name="destination">The stream to which data will be copied.</param>
        /// <param name="bufferSize">The size of the buffer. The value must be greater than zero.</param>
        /// <param name="copyLimit">
        /// The maximum number of bytes to copy. The default value is -1. If the value is negative,
        /// all remaining data in <paramref name="source"/> will be copied.
        /// </param>
        /// <returns>The total number of bytes copied into <paramref name="destination"/>.</returns>
        public static long CopyData(this Stream source, Stream destination, int bufferSize,
            long copyLimit = -1)
        {
            CheckCopyDataArguments(source, destination, bufferSize);

            if (copyLimit == 0)
            {
                return 0;
            }

            var copied = 0L;
            var buffer = ArrayPool<byte>.Shared.Rent(bufferSize);
            try
            {
                while (true)
                {
                    var readLength = CreateReadLength(copyLimit, copied, buffer);
                    if (readLength == 0)
                    {
                        break;
                    }

                    var readSpan = new Span<byte>(buffer, 0, readLength);
                    var readCount = source.Read(readSpan);
                    if (readCount == 0)
                    {
                        break;
                    }

                    var writeSpan = new ReadOnlySpan<byte>(buffer, 0, readCount);
                    destination.Write(writeSpan);
                    copied += readCount;
                }
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(buffer);
            }

            return copied;
        }

        /// <summary>
        /// Reads a sequence of bytes from a source stream and writes them to a destination stream.
        /// </summary>
        /// <remarks>This method using the default buffer size (81920 bytes).</remarks>
        /// <param name="source">The stream from which data will be copied.</param>
        /// <param name="destination">The stream to which data will be copied.</param>
        /// <param name="copyLimit">
        /// The maximum number of bytes to copy. The default value is -1. If the value is negative,
        /// all remaining data in <paramref name="source"/> will be copied.
        /// </param>
        /// <returns>The total number of bytes copied into <paramref name="destination"/>.</returns>
        public static long CopyData(this Stream source, Stream destination,
            long copyLimit = -1)
        {
            var bufferSize = source.GetCopyDataBufferSize();
            return source.CopyData(destination, bufferSize, copyLimit);
        }

        /// <summary>
        /// Asynchronously reads a sequence of bytes from a source stream and writes them to a
        /// destination stream.
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous copy operation. The result value contains the
        /// total number of bytes copied into <paramref name="destination"/>.
        /// </returns>
        /// <inheritdoc cref="CopyData(Stream, Stream, int, long)"/>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        /// <param name="bufferSize"></param>
        /// <param name="copyLimit"></param>
        /// <param name="cancellationToken">
        /// The token to monitor for cancellation requests. The default value is <see cref="CancellationToken.None"/>.
        /// </param>
        public static async ValueTask<long> CopyDataAsync(this Stream source, Stream destination, int bufferSize,
            long copyLimit = -1, CancellationToken cancellationToken = default)
        {
            CheckCopyDataArguments(source, destination, bufferSize);

            if (copyLimit == 0)
            {
                return 0;
            }

            var copied = 0L;
            var buffer = ArrayPool<byte>.Shared.Rent(bufferSize);
            try
            {
                while (true)
                {
                    var readLength = CreateReadLength(copyLimit, copied, buffer);
                    if (readLength == 0)
                    {
                        break;
                    }

                    var readMemory = new Memory<byte>(buffer, 0, readLength);
                    var readCount = await source.ReadAsync(readMemory, cancellationToken)
                        .ConfigureAwait(continueOnCapturedContext: false);
                    if (readCount == 0)
                    {
                        break;
                    }

                    var writeMemory = new ReadOnlyMemory<byte>(buffer, 0, readCount);
                    await destination.WriteAsync(writeMemory, cancellationToken)
                        .ConfigureAwait(continueOnCapturedContext: false);
                    copied += readCount;
                }
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(buffer);
            }

            return copied;
        }

        /// <summary>
        /// Asynchronously reads a sequence of bytes from a source stream and writes them to a
        /// destination stream.
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous copy operation. The result value contains the
        /// total number of bytes copied into <paramref name="destination"/>.
        /// </returns>
        /// <inheritdoc cref="CopyData(Stream, Stream, long)"/>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        /// <param name="copyLimit"></param>
        /// <param name="cancellationToken">
        /// The token to monitor for cancellation requests. The default value is <see cref="CancellationToken.None"/>.
        /// </param>
        public static ValueTask<long> CopyDataAsync(this Stream source, Stream destination,
            long copyLimit = -1, CancellationToken cancellationToken = default)
        {
            var bufferSize = source.GetCopyDataBufferSize();
            return source.CopyDataAsync(destination, bufferSize, copyLimit, cancellationToken);
        }

        private static void CheckCopyDataArguments(Stream source, Stream destination, int bufferSize)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (destination is null)
            {
                throw new ArgumentNullException(nameof(destination));
            }

            if (!source.CanRead)
            {
                throw new NotSupportedException(Error_NonReadableSourceStream);
            }

            if (!destination.CanWrite)
            {
                throw new NotSupportedException(Error_NonWritableDestinationStream);
            }

            if (bufferSize <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(bufferSize), Error_InvalidBufferSize);
            }
        }

        private static int GetCopyDataBufferSize(this Stream stream, int defaultSize = 81920)
        {
            if (stream.CanSeek)
            {
                var length = stream.Length;
                var position = stream.Position;
                if (length <= position)
                {
                    return 1;
                }
                else
                {
                    var bytesLeft = length - position;
                    if (bytesLeft < defaultSize)
                    {
                        return (int)bytesLeft;
                    }
                }
            }

            return defaultSize;
        }

        private static int CreateReadLength(long copyLimit, long copied, byte[] buffer)
        {
            if (copyLimit >= 0)
            {
                var left = copyLimit - copied;
                if (left == 0)
                {
                    return 0;
                }
                if (left < buffer.Length)
                {
                    return (int)left;
                }
            }

            return buffer.Length;
        }
    }
}