// Licensed under the MIT License. See LICENSE file for full license information.

#pragma warning disable CS1573

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Motormoth.Extensions.IO
{
    /// <summary>
    /// Provides the base class for stream decorators.
    /// </summary>
    public abstract class StreamDecorator : Stream
    {
        /// <summary>
        /// The stream on which the current stream decorator is based.
        /// </summary>
        private readonly Stream underlyingStream;

        /// <summary>
        /// Initializes a new instance of the <see cref="StreamDecorator"/> class based on the
        /// specified stream.
        /// </summary>
        /// <param name="stream">The stream on which the current stream decorator will be based.</param>
        protected StreamDecorator(Stream stream)
        {
            if (stream is null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            this.underlyingStream = stream;
        }

        /// <summary>
        /// Gets or sets a value, in milliseconds, that determines how long the underlying stream
        /// will attempt to read before timing out.
        /// </summary>
        public override int ReadTimeout
        {
            get => this.underlyingStream.ReadTimeout;
            set => this.underlyingStream.ReadTimeout = value;
        }

        /// <summary>
        /// Gets or sets a value, in milliseconds, that determines how long the underlying stream
        /// will attempt to write before timing out.
        /// </summary>
        public override int WriteTimeout
        {
            get => this.underlyingStream.WriteTimeout;
            set => this.underlyingStream.WriteTimeout = value;
        }

        /// <summary>
        /// Gets or sets the position within the underlying stream.
        /// </summary>
        public override long Position
        {
            get => this.underlyingStream.Position;
            set => this.underlyingStream.Position = value;
        }

        /// <summary>
        /// Gets a value indicating whether the underlying stream supports seeking.
        /// </summary>
        public override bool CanSeek => this.underlyingStream.CanSeek;

        /// <summary>
        /// Gets a value indicating whether the underlying stream supports reading.
        /// </summary>
        public override bool CanRead => this.underlyingStream.CanRead;

        /// <summary>
        /// Gets a value indicating whether the underlying stream supports writing.
        /// </summary>
        public override bool CanWrite => this.underlyingStream.CanWrite;

        /// <summary>
        /// Gets a value indicating whether the underlying stream can time out.
        /// </summary>
        public override bool CanTimeout => this.underlyingStream.CanTimeout;

        /// <summary>
        /// Gets the length in bytes of the underlying stream.
        /// </summary>
        public override long Length => this.underlyingStream.Length;

        /// <summary>
        /// Sets the position within the underlying stream.
        /// </summary>
        /// <param name="offset"><inheritdoc/></param>
        /// <param name="origin"><inheritdoc/></param>
        /// <returns>The new position within the underlying stream.</returns>
        public override long Seek(long offset, SeekOrigin origin)
        {
            return this.underlyingStream.Seek(offset, origin);
        }

        /// <summary>
        /// Reads a sequence of bytes from the underlying stream and advances the position within
        /// this stream by the number of bytes read.
        /// </summary>
        /// <param name="buffer">The buffer to read the data into.</param>
        /// <param name="offset">
        /// The zero-based byte offset in buffer at which to begin writing the data.
        /// </param>
        /// <param name="count">The maximum number of bytes to read.</param>
        /// <returns>
        /// The total number of bytes read into the <paramref name="buffer"/>. This can be less than
        /// the number of bytes requested if that many bytes are not currently available, or zero
        /// (0) if the end of the undelying stream has been reached.
        /// </returns>
        public override int Read(byte[] buffer, int offset, int count)
        {
            return this.underlyingStream.Read(buffer, offset, count);
        }

        /// <inheritdoc cref="Read(byte[], int, int)"/>
        public override int Read(Span<byte> buffer)
        {
            return this.underlyingStream.Read(buffer);
        }

        /// <summary>
        /// Begins an asynchronous read operation. (Consider using <see cref="ReadAsync(byte[], int,
        /// int, CancellationToken)"/> instead.)
        /// </summary>
        /// <param name="callback">
        /// An optional asynchronous callback, to be called when the read is complete.
        /// </param>
        /// <param name="state">
        /// A user-provided object that distinguishes this particular asynchronous read request from
        /// other requests.
        /// </param>
        /// <returns>
        /// An <see cref="IAsyncResult"/> that represents the asynchronous read, which could still
        /// be pending.
        /// </returns>
        /// <inheritdoc cref="Read(byte[], int, int)"/>
        public override IAsyncResult BeginRead(byte[] buffer, int offset, int count,
            AsyncCallback callback, object state)
        {
            return this.underlyingStream.BeginRead(buffer, offset, count, callback, state);
        }

        /// <summary>
        /// Waits for the pending asynchronous read to complete. (Consider using <see
        /// cref="ReadAsync(byte[], int, int, CancellationToken)"/> instead.)
        /// </summary>
        /// <param name="asyncResult">The reference to the pending asynchronous request to finish.</param>
        /// <returns>
        /// The total number of bytes read from the underlying stream. This can be less than the
        /// number of bytes requested if that many bytes are not currently available, or zero (0) if
        /// the end of the undelying stream has been reached.
        /// </returns>
        public override int EndRead(IAsyncResult asyncResult)
        {
            return this.underlyingStream.EndRead(asyncResult);
        }

        /// <summary>
        /// Asynchronously reads a sequence of bytes from the unserlying stream, advances the
        /// position within this stream by the number of bytes read, and monitors cancellation requests.
        /// </summary>
        /// <param name="cancellationToken">
        /// The token to monitor for cancellation requests. The default value is <see cref="CancellationToken.None"/>.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The result value contains the
        /// total number of bytes read into the buffer.
        /// </returns>
        /// <inheritdoc cref="Read(byte[], int, int)"/>
        public override Task<int> ReadAsync(byte[] buffer, int offset, int count,
            CancellationToken cancellationToken)
        {
            return this.underlyingStream.ReadAsync(buffer, offset, count, cancellationToken);
        }

        /// <inheritdoc cref="ReadAsync(byte[], int, int, CancellationToken)"/>
        public override ValueTask<int> ReadAsync(Memory<byte> buffer,
            CancellationToken cancellationToken = default)
        {
            return this.underlyingStream.ReadAsync(buffer, cancellationToken);
        }

        /// <summary>
        /// Reads a byte from the underlying stream and advances the position within this stream by
        /// one byte.
        /// </summary>
        /// <returns>
        /// The unsigned byte cast to an <see cref="int"/>, or -1 if at the end of the underlying stream.
        /// </returns>
        public override int ReadByte()
        {
            return this.underlyingStream.ReadByte();
        }

        /// <summary>
        /// Writes a sequence of bytes to the underlying stream and advances the current position
        /// within this stream by the number of bytes written.
        /// </summary>
        /// <param name="buffer">The buffer to write data from.</param>
        /// <param name="offset">
        /// The zero-based byte offset in buffer at which to begin reading the data.
        /// </param>
        /// <param name="count">The maximum number of bytes to write.</param>
        public override void Write(byte[] buffer, int offset, int count)
        {
            this.underlyingStream.Write(buffer, offset, count);
        }

        /// <inheritdoc cref="Write(byte[], int, int)"/>
        public override void Write(ReadOnlySpan<byte> buffer)
        {
            this.underlyingStream.Write(buffer);
        }

        /// <summary>
        /// Begins an asynchronous read operation. (Consider using <see cref="WriteAsync(byte[],
        /// int, int, CancellationToken)"/> instead.)
        /// </summary>
        /// <param name="callback">
        /// An optional asynchronous callback, to be called when the write is complete.
        /// </param>
        /// <param name="state">
        /// A user-provided object that distinguishes this particular asynchronous write request
        /// from other requests.
        /// </param>
        /// <returns>
        /// An <see cref="IAsyncResult"/> that represents the asynchronous write, which could still
        /// be pending.
        /// </returns>
        /// <inheritdoc cref="Write(byte[], int, int)"/>
        public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count,
            AsyncCallback callback, object state)
        {
            return this.underlyingStream.BeginWrite(buffer, offset, count, callback, state);
        }

        /// <summary>
        /// Waits for the pending asynchronous write to complete. (Consider using <see
        /// cref="WriteAsync(byte[], int, int, CancellationToken)"/> instead.)
        /// </summary>
        /// <param name="asyncResult">The reference to the pending asynchronous request to finish.</param>
        public override void EndWrite(IAsyncResult asyncResult)
        {
            this.underlyingStream.EndWrite(asyncResult);
        }

        /// <summary>
        /// Asynchronously writes a sequence of bytes to the underlying stream, advances the
        /// position within this stream by the number of bytes read, and monitors cancellation requests.
        /// </summary>
        /// <param name="cancellationToken">
        /// The token to monitor for cancellation requests. The default value is <see cref="CancellationToken.None"/>.
        /// </param>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        /// <inheritdoc cref="Write(byte[], int, int)"/>
        public override Task WriteAsync(byte[] buffer, int offset, int count,
            CancellationToken cancellationToken)
        {
            return this.underlyingStream.WriteAsync(buffer, offset, count, cancellationToken);
        }

        /// <inheritdoc cref="WriteAsync(byte[], int, int, CancellationToken)"/>
        public override ValueTask WriteAsync(ReadOnlyMemory<byte> buffer,
            CancellationToken cancellationToken = default)
        {
            return this.underlyingStream.WriteAsync(buffer, cancellationToken);
        }

        /// <summary>
        /// Writes a byte to the current position in the underlying stream and advances the position
        /// within this stream by one byte.
        /// </summary>
        /// <param name="value">The byte to write to the underlying stream.</param>
        public override void WriteByte(byte value)
        {
            this.underlyingStream.WriteByte(value);
        }

        /// <summary>
        /// Clears all buffers for the underlying stream and causes any buffered data to be written
        /// to the underlying device.
        /// </summary>
        public override void Flush()
        {
            this.underlyingStream.Flush();
        }

        /// <summary>
        /// Asynchronously clears all buffers for the underlying stream, causes any buffered data to
        /// be written to the underlying device, and monitors cancellation requests.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns>A task that represents the asynchronous flush operation.</returns>
        public override Task FlushAsync(
            CancellationToken cancellationToken)
        {
            return this.underlyingStream.FlushAsync(cancellationToken);
        }

        /// <summary>
        /// Reads the bytes from the underlying stream and writes them to another stream, using a
        /// specified buffer size.
        /// </summary>
        /// <param name="destination">
        /// The stream to which the contents of the underlying stream will be copied.
        /// </param>
        /// <param name="bufferSize">The size of the buffer. This value must be greater than zero.</param>
        public override void CopyTo(Stream destination, int bufferSize)
        {
            this.underlyingStream.CopyTo(destination, bufferSize);
        }

        /// <summary>
        /// Asynchronously reads the bytes from the underlying stream and writes them to another stream, using a specified buffer size and cancellation token.
        /// </summary>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A task that represents the asynchronous copy operation.</returns>
        ///<inheritdoc cref="CopyTo(Stream, int)"/>
        public override Task CopyToAsync(Stream destination, int bufferSize,
            CancellationToken cancellationToken)
        {
            return this.underlyingStream.CopyToAsync(destination, bufferSize, cancellationToken);
        }

        /// <summary>
        /// Sets the length of the underlying stream.
        /// </summary>
        /// <param name="value">The desired length of the underlying stream in bytes.</param>
        public override void SetLength(long value)
        {
            this.underlyingStream.SetLength(value);
        }

        /// <summary>
        /// Closes the underlying stream and releases any resources associated with it. Instead of
        /// calling this method, ensure that the stream decorator is properly disposed.
        /// </summary>
        public override void Close()
        {
            this.underlyingStream.Close();
        }
    }
}