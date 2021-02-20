// Licensed under the MIT License. See LICENSE file for full license information.

using System;
using System.IO;

using Motormoth.Extensions.IO;

using Xunit;

namespace Motormoth.Extensions.Tests.IO
{
    partial class FileAssemblerTest
    {
        [Fact]
        public void AddFilePart_FilePartNull_ThrowsArgumentNullException()
        {
            var fileAssembler = CreateFileAssembler();
            var workspaceId = CreateDefaultWorkspaceId();
            var filePart = CreateNullStream();
            var filePartNumber = CreateDefaultFilePartNumber();

            void testAction()
            {
                _ = fileAssembler.AddFilePart(workspaceId, filePart, filePartNumber);
            }

            _ = Assert.Throws<ArgumentNullException>(testAction);
        }

        [Theory]
        [InlineData(FileAssembler.MinFilePartNumber - 1)]
        [InlineData(FileAssembler.MaxFilePartNumber + 1)]
        public void AddFilePart_FilePartNumberInvalid_ThrowsArgumentOutOfRangeException(int filePartNumber)
        {
            var fileAssembler = CreateFileAssembler();
            var workspaceId = CreateDefaultWorkspaceId();
            using var filePart = CreateDefaultFilePart();

            void testAction()
            {
                _ = fileAssembler.AddFilePart(workspaceId, filePart, filePartNumber);
            }

            _ = Assert.Throws<ArgumentOutOfRangeException>(testAction);
        }

        [Fact]
        public void AddFilePart_WorkSpaceExsits_ReturnsTrue()
        {
            var fileAssembler = CreateFileAssembler();
            using var filePart = CreateDefaultFilePart();
            var filePartNumber = CreateDefaultFilePartNumber();

            var workspaceId = fileAssembler.CreateWorkspace();
            var workspaceFound = fileAssembler.AddFilePart(workspaceId, filePart, filePartNumber);

            Assert.True(workspaceFound);
        }

        [Fact]
        public void AddFilePart_WorkSpaceNotExsits_ReturnsFalse()
        {
            var fileAssembler = CreateFileAssembler();
            var workspaceId = CreateDefaultWorkspaceId();
            using var filePart = CreateDefaultFilePart();
            var filePartNumber = CreateDefaultFilePartNumber();

            var workspaceFound = fileAssembler.AddFilePart(workspaceId, filePart, filePartNumber);

            Assert.False(workspaceFound);
        }

        [Theory]
        [InlineData(new byte[0], FileAssembler.MinFilePartNumber)]
        [InlineData(new byte[0], FileAssembler.MaxFilePartNumber)]
        [InlineData(new byte[1] { 1 }, FileAssembler.MinFilePartNumber)]
        [InlineData(new byte[1] { 1 }, FileAssembler.MaxFilePartNumber)]
        public void AddFilePart_CorrectParameters_CreatesFileWithExpectedContent(byte[] filePartData, int filePartNumber)
        {
            var fileAssembler = CreateFileAssembler();
            using var filePart = new MemoryStream(filePartData, writable: false);

            var workspaceId = fileAssembler.CreateWorkspace();
            _ = fileAssembler.AddFilePart(workspaceId, filePart, filePartNumber);
            var filePartPath = GetFilePartPath(workspaceId, filePartNumber);
            var filePartFileData = File.ReadAllBytes(filePartPath);

            Assert.Equal(filePartData, filePartFileData);
        }
    }
}