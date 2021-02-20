// Licensed under the MIT License. See LICENSE file for full license information.

using System;
using System.IO;
using System.Threading.Tasks;

using Xunit;

namespace Motormoth.Extensions.Tests.IO
{
    partial class FileAssemblerTest
    {
        [Fact]
        public async Task AssembleFileAsync_ProductPathNull_ThrowsArgumentNullException()
        {
            var fileAssembler = CreateFileAssembler();
            var workspaceId = CreateDefaultWorkspaceId();
            var productPath = CreateNullPath();

            async Task testAction()
            {
                _ = await fileAssembler.AssembleFileAsync(workspaceId, productPath);
            }

            _ = await Assert.ThrowsAsync<ArgumentNullException>(testAction);
        }

        [Fact]
        public async Task AssembleFileAsync_WorkSpaceExsits_ReturnsTrue()
        {
            var fileAssembler = CreateFileAssembler();

            var workspaceId = fileAssembler.CreateWorkspace();
            var productPath = GetProductPath(workspaceId);
            var workspaceFound = await fileAssembler.AssembleFileAsync(workspaceId, productPath);

            Assert.True(workspaceFound);
        }

        [Fact]
        public async Task AssembleFileAsync_WorkSpaceNotExsits_ReturnsFalse()
        {
            var fileAssembler = CreateFileAssembler();
            var workspaceId = CreateDefaultWorkspaceId();

            var productPath = GetProductPath(workspaceId);
            var workspaceFound = await fileAssembler.AssembleFileAsync(workspaceId, productPath);

            Assert.False(workspaceFound);
        }

        [Fact]
        public async Task AssembleFileAsync_NoParts_CreatesEmptyFile()
        {
            var fileAssembler = CreateFileAssembler();

            var workspaceId = fileAssembler.CreateWorkspace();
            var productPath = GetProductPath(workspaceId);
            _ = await fileAssembler.AssembleFileAsync(workspaceId, productPath);
            var productFileData = await File.ReadAllBytesAsync(productPath);

            Assert.Equal(Array.Empty<byte>(), productFileData);
        }

        [Theory]
        [InlineData(new byte[0])]
        [InlineData(new byte[1] { 1 })]
        public async Task AssembleFileAsync_OnePart_CreatesFileWithExpectedContent(byte[] filePartData)
        {
            var fileAssembler = CreateFileAssembler();
            await using var filePart = new MemoryStream(filePartData, writable: false);
            var filePartNumber = 1;

            var workspaceId = fileAssembler.CreateWorkspace();
            var productPath = GetProductPath(workspaceId);
            _ = await fileAssembler.AddFilePartAsync(workspaceId, filePart, filePartNumber);
            _ = await fileAssembler.AssembleFileAsync(workspaceId, productPath);
            var productFileData = await File.ReadAllBytesAsync(productPath);

            Assert.Equal(filePartData, productFileData);
        }

        [Theory]
        [InlineData(new byte[0], new byte[0], new byte[0])]
        [InlineData(new byte[1] { 1 }, new byte[1] { 2 }, new byte[2] { 1, 2 })]
        [InlineData(new byte[1] { 2 }, new byte[1] { 1 }, new byte[2] { 2, 1 })]
        public async Task AssembleFileAsync_TwoParts_CreatesFileWithExpectedContent(byte[] filePart1Data, byte[] filePart2Data, byte[] expectedProductFileData)
        {
            var fileAssembler = CreateFileAssembler();
            await using var filePart1 = new MemoryStream(filePart1Data, writable: false);
            await using var filePart2 = new MemoryStream(filePart2Data, writable: false);
            var filePart1Number = 1;
            var filePart2Number = 2;

            var workspaceId = fileAssembler.CreateWorkspace();
            var productPath = GetProductPath(workspaceId);
            _ = await fileAssembler.AddFilePartAsync(workspaceId, filePart1, filePart1Number);
            _ = await fileAssembler.AddFilePartAsync(workspaceId, filePart2, filePart2Number);
            _ = await fileAssembler.AssembleFileAsync(workspaceId, productPath);
            var productFileData = await File.ReadAllBytesAsync(productPath);

            Assert.Equal(expectedProductFileData, productFileData);
        }
    }
}