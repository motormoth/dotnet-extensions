// Licensed under the MIT License. See LICENSE file for full license information.

using System;
using System.IO;

namespace Motormoth.Extensions.Tests.IO
{
    public sealed class DirectoryFixture : IDisposable
    {
        public const string DirectoryName = "TestDirectory";

        private readonly DirectoryInfo directory;

        private bool isDisposed;

        public DirectoryFixture()
        {
            this.directory = Directory.CreateDirectory(DirectoryName);
        }

        ~DirectoryFixture()
        {
            Dispose(disposing: false);
        }

        public string Path => this.directory.FullName;

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!this.isDisposed)
            {
                if (disposing)
                {
                }

                if (this.directory.Exists)
                {
                    try
                    {
                        this.directory.Delete(recursive: true);
                    }
                    catch (DirectoryNotFoundException)
                    {
                    }
                }

                this.isDisposed = true;
            }
        }
    }
}