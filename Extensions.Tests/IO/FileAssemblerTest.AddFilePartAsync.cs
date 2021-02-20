// Licensed under the MIT License. See LICENSE file for full license information.

using System;
using System.IO;
using System.Threading.Tasks;

using Motormoth.Extensions.IO;

using Xunit;

namespace Motormoth.Extensions.Tests.IO
{
    partial class FileAssemblerTest
    {
        [Fact]
        public async Task AddFilePartAsync_FilePartNull_ThrowsArgumentNullException()
        {
            var fileAssembler = CreateFileAssembler();
            var workspaceId = CreateDefaultWorkspaceId();
            var filePart = CreateNullStream();
            var filePartNumber = CreateDefaultFilePartNumber();

            async Task testAction()
            {
                _ = await fileAssembler.AddFilePartAsync(workspaceId, filePart, filePartNumber);
            }

            _ = await Assert.ThrowsAsync<ArgumentNullException>(testAction);
        }

        [Theory]
        [InlineData(FileAssembler.MinFilePartNumber - 1)]
        [InlineData(FileAssembler.MaxFilePartNumber + 1)]
        public async Task AddFilePartAsync_FilePartNumberInvalid_ThrowsArgumentOutOfRangeException(int filePartNumber)
        {
            var fileAssembler = CreateFileAssembler();
            var workspaceId = CreateDefaultWorkspaceId();
            using var filePart = CreateDefaultFilePart();

            async Task testAction()
            {
                _ = await fileAssembler.AddFilePartAsync(workspaceId, filePart, filePartNumber);
            }

            _ = await Assert.ThrowsAsync<ArgumentOutOfRangeException>(testAction);
        }

        [Fact]
        public async Task AddFilePartAsync_WorkSpaceExsits_ReturnsTrue()
        {
            var fileAssembler = CreateFileAssembler();
            using var filePart = CreateDefaultFilePart();
            var filePartNumber = CreateDefaultFilePartNumber();

            var workspaceId = fileAssembler.CreateWorkspace();
            var workspaceFound = await fileAssembler.AddFilePartAsync(workspaceId, filePart, filePartNumber);

            Assert.True(workspaceFound);
        }

        [Fact]
        public async Task AddFilePartAsync_WorkSpaceNotExsits_ReturnsFalse()
        {
            var fileAssembler = CreateFileAssembler();
            var workspaceId = CreateDefaultWorkspaceId();
            using var filePart = CreateDefaultFilePart();
            var filePartNumber = CreateDefaultFilePartNumber();

            var workspaceFound = await fileAssembler.AddFilePartAsync(workspaceId, filePart, filePartNumber);

            Assert.False(workspaceFound);
        }

        [Theory]
        [InlineData(new byte[0], FileAssembler.MinFilePartNumber)]
        [InlineData(new byte[0], FileAssembler.MaxFilePartNumber)]
        [InlineData(new byte[1] { 1 }, FileAssembler.MinFilePartNumber)]
        [InlineData(new byte[1] { 1 }, FileAssembler.MaxFilePartNumber)]
        public async Task AddFilePartAsync_CorrectParameters_CreatesFileWithExpectedContent(byte[] filePartData, int filePartNumber)
        {
            var fileAssembler = CreateFileAssembler();
            await using var filePart = new MemoryStream(filePartData, writable: false);

            var workspaceId = fileAssembler.CreateWorkspace();
            _ = await fileAssembler.AddFilePartAsync(workspaceId, filePart, filePartNumber);
            var filePartPath = GetFilePartPath(workspaceId, filePartNumber);
            var filePartFileData = await File.ReadAllBytesAsync(filePartPath);

            Assert.Equal(filePartData, filePartFileData);
        }
    }
}