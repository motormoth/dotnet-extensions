// Licensed under the MIT License. See LICENSE file for full license information.

using System;
using System.IO;

using Motormoth.Extensions.IO;

using Xunit;

namespace Motormoth.Extensions.Tests.IO
{
    public sealed partial class FileAssemblerTest : IClassFixture<DirectoryFixture>
    {
        private readonly DirectoryFixture directoryFixture;

        public FileAssemblerTest(DirectoryFixture directoryFixture)
        {
            this.directoryFixture = directoryFixture;
        }

        [Fact]
        public void Ctor_RootPathNull_ThrowsArgumentNullException()
        {
            var rootPath = CreateNullPath();

            void testAction()
            {
                _ = new FileAssembler(rootPath);
            }

            _ = Assert.Throws<ArgumentNullException>(testAction);
        }

        private static string CreateNullPath()
        {
            return null;
        }

        private static Stream CreateNullStream()
        {
            return null;
        }

        private static Guid CreateDefaultWorkspaceId()
        {
            return Guid.Empty;
        }

        private static MemoryStream CreateDefaultFilePart()
        {
            return new(Array.Empty<byte>(), writable: false);
        }

        private static int CreateDefaultFilePartNumber()
        {
            return FileAssembler.MinFilePartNumber;
        }

        private FileAssembler CreateFileAssembler()
        {
            return new FileAssembler(this.directoryFixture.Path);
        }

        private string GetWorkspacePath(Guid workspaceId)
        {
            return Path.Combine(this.directoryFixture.Path, $"{workspaceId:N}");
        }

        private string GetFilePartPath(Guid workspaceId, int filePartNumber)
        {
            return Path.Combine(this.directoryFixture.Path, $"{workspaceId:N}", $"{filePartNumber:D5}.{FileAssembler.FilePartFileExt}");
        }

        private string GetProductPath(Guid workspaceId)
        {
            const string productName = "product";
            return Path.Combine(this.directoryFixture.Path, $"{workspaceId:N}", productName);
        }
    }
}