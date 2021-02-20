// Licensed under the MIT License. See LICENSE file for full license information.

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Motormoth.Extensions.IO
{
    /// <summary>
    /// Provides the read-only stream decorator.
    /// </summary>
    public partial class ReadOnlyStreamDecorator : StreamDecorator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlyStreamDecorator"/> class based on
        /// the specified stream.
        /// </summary>
        /// <param name="stream">
        /// The stream on which the current read-only stream decorator will be based.
        /// </param>
        public ReadOnlyStreamDecorator(Stream stream)
            : base(stream)
        {
        }

        /// <inheritdoc/>
        public override bool CanWrite => false;

        /// <remarks>This method always throws an <see cref="NotSupportedException"/>.</remarks>
        /// <exception cref="NotSupportedException">Always thrown.</exception>
        /// <inheritdoc/>
        public override int WriteTimeout => throw new NotSupportedException(ExceptionMessage_ReadOnlyStream);

        /// <remarks>This method always throws an <see cref="NotSupportedException"/>.</remarks>
        /// <exception cref="NotSupportedException">Always thrown.</exception>
        /// <inheritdoc/>
        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException(ExceptionMessage_ReadOnlyStream);
        }

        /// <remarks>This method always throws an <see cref="NotSupportedException"/>.</remarks>
        /// <exception cref="NotSupportedException">Always thrown.</exception>
        /// <inheritdoc/>
        public override void Write(ReadOnlySpan<byte> buffer)
        {
            throw new NotSupportedException(ExceptionMessage_ReadOnlyStream);
        }

        /// <remarks>This method always throws an <see cref="NotSupportedException"/>.</remarks>
        /// <exception cref="NotSupportedException">Always thrown.</exception>
        /// <inheritdoc/>
        public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            throw new NotSupportedException(ExceptionMessage_ReadOnlyStream);
        }

        /// <remarks>This method always throws an <see cref="NotSupportedException"/>.</remarks>
        /// <exception cref="NotSupportedException">Always thrown.</exception>
        /// <inheritdoc/>
        public override void EndWrite(IAsyncResult asyncResult)
        {
            throw new NotSupportedException(ExceptionMessage_ReadOnlyStream);
        }

        /// <remarks>This method always throws an <see cref="NotSupportedException"/>.</remarks>
        /// <exception cref="NotSupportedException">Always thrown.</exception>
        /// <inheritdoc/>
        public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            throw new NotSupportedException(ExceptionMessage_ReadOnlyStream);
        }

        /// <remarks>This method always throws an <see cref="NotSupportedException"/>.</remarks>
        /// <exception cref="NotSupportedException">Always thrown.</exception>
        /// <inheritdoc/>
        public override ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException(ExceptionMessage_ReadOnlyStream);
        }

        /// <remarks>This method always throws an <see cref="NotSupportedException"/>.</remarks>
        /// <exception cref="NotSupportedException">Always thrown.</exception>
        /// <inheritdoc/>
        public override void WriteByte(byte value)
        {
            throw new NotSupportedException(ExceptionMessage_ReadOnlyStream);
        }

        /// <remarks>This method always throws an <see cref="NotSupportedException"/>.</remarks>
        /// <exception cref="NotSupportedException">Always thrown.</exception>
        /// <inheritdoc/>
        public override void Flush()
        {
            throw new NotSupportedException(ExceptionMessage_ReadOnlyStream);
        }

        /// <remarks>This method always throws an <see cref="NotSupportedException"/>.</remarks>
        /// <exception cref="NotSupportedException">Always thrown.</exception>
        /// <inheritdoc/>
        public override Task FlushAsync(CancellationToken cancellationToken)
        {
            throw new NotSupportedException(ExceptionMessage_ReadOnlyStream);
        }

        /// <remarks>This method always throws an <see cref="NotSupportedException"/>.</remarks>
        /// <exception cref="NotSupportedException">Always thrown.</exception>
        /// <inheritdoc/>
        public override void SetLength(long value)
        {
            throw new NotSupportedException(ExceptionMessage_ReadOnlyStream);
        }
    }
}