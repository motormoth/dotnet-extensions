// Licensed under the MIT License. See LICENSE file for full license information.

using System;
using System.IO;

namespace Motormoth.Extensions.Tests.IO
{
    public sealed class FakeStream : Stream
    {
        private readonly bool canRead;

        private readonly bool canWrite;

        public FakeStream(bool canRead = true, bool canWrite = true)
        {
            this.canRead = canRead;
            this.canWrite = canWrite;
        }

        public override bool CanSeek => throw new NotImplementedException();

        public override bool CanRead => this.canRead;

        public override bool CanWrite => this.canWrite;

        public override long Length => throw new NotImplementedException();

        public override long Position
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        public override void Flush()
        {
            throw new NotImplementedException();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException();
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }
    }
}