// Licensed under the MIT License. See LICENSE file for full license information.

using System;
using System.IO;

using Xunit;

namespace Motormoth.Extensions.Tests.IO
{
    partial class FileAssemblerTest
    {
        [Fact]
        public void AssembleFile_ProductPathNull_ThrowsArgumentNullException()
        {
            var fileAssembler = CreateFileAssembler();
            var workspaceId = CreateDefaultWorkspaceId();
            var productPath = CreateNullPath();

            void testAction()
            {
                _ = fileAssembler.AssembleFile(workspaceId, productPath);
            }

            _ = Assert.Throws<ArgumentNullException>(testAction);
        }

        [Fact]
        public void AssembleFile_WorkSpaceExsits_ReturnsTrue()
        {
            var fileAssembler = CreateFileAssembler();

            var workspaceId = fileAssembler.CreateWorkspace();
            var productPath = GetProductPath(workspaceId);
            var workspaceFound = fileAssembler.AssembleFile(workspaceId, productPath);

            Assert.True(workspaceFound);
        }

        [Fact]
        public void AssembleFile_WorkSpaceNotExsits_ReturnsFalse()
        {
            var fileAssembler = CreateFileAssembler();
            var workspaceId = CreateDefaultWorkspaceId();

            var productPath = GetProductPath(workspaceId);
            var workspaceFound = fileAssembler.AssembleFile(workspaceId, productPath);

            Assert.False(workspaceFound);
        }

        [Fact]
        public void AssembleFile_NoParts_CreatesEmptyFile()
        {
            var fileAssembler = CreateFileAssembler();

            var workspaceId = fileAssembler.CreateWorkspace();
            var productPath = GetProductPath(workspaceId);
            _ = fileAssembler.AssembleFile(workspaceId, productPath);
            var productFileData = File.ReadAllBytes(productPath);

            Assert.Equal(Array.Empty<byte>(), productFileData);
        }

        [Theory]
        [InlineData(new byte[0])]
        [InlineData(new byte[1] { 1 })]
        public void AssembleFile_OnePart_CreatesFileWithExpectedContent(byte[] filePartData)
        {
            var fileAssembler = CreateFileAssembler();
            using var filePart = new MemoryStream(filePartData, writable: false);
            var filePartNumber = 1;

            var workspaceId = fileAssembler.CreateWorkspace();
            var productPath = GetProductPath(workspaceId);
            _ = fileAssembler.AddFilePart(workspaceId, filePart, filePartNumber);
            _ = fileAssembler.AssembleFile(workspaceId, productPath);
            var productFileData = File.ReadAllBytes(productPath);

            Assert.Equal(filePartData, productFileData);
        }

        [Theory]
        [InlineData(new byte[0], new byte[0], new byte[0])]
        [InlineData(new byte[1] { 1 }, new byte[1] { 2 }, new byte[2] { 1, 2 })]
        [InlineData(new byte[1] { 2 }, new byte[1] { 1 }, new byte[2] { 2, 1 })]
        public void AssembleFile_TwoParts_CreatesFileWithExpectedContent(byte[] filePart1Data, byte[] filePart2Data, byte[] expectedProductFileData)
        {
            var fileAssembler = CreateFileAssembler();
            using var filePart1 = new MemoryStream(filePart1Data, writable: false);
            using var filePart2 = new MemoryStream(filePart2Data, writable: false);
            var filePart1Number = 1;
            var filePart2Number = 2;

            var workspaceId = fileAssembler.CreateWorkspace();
            var productPath = GetProductPath(workspaceId);
            _ = fileAssembler.AddFilePart(workspaceId, filePart1, filePart1Number);
            _ = fileAssembler.AddFilePart(workspaceId, filePart2, filePart2Number);
            _ = fileAssembler.AssembleFile(workspaceId, productPath);
            var productFileData = File.ReadAllBytes(productPath);

            Assert.Equal(expectedProductFileData, productFileData);
        }
    }
}