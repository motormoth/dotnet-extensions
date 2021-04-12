// Licensed under the MIT License. See LICENSE file for full license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace Motormoth.Extensions.Tests.IO
{
    partial class FileAssemblerTest
    {
        [Fact]
        public void AssembleAt_FilePathNull_ThrowsArgumentNullException()
        {
            var fileAssembler = CreateDefaultFileAssembler();
            var workspaceId = CreateDefaultWorkspaceId();
            var filePath = (string)null;

            void testAction()
            {
                fileAssembler.AssembleAt(workspaceId, filePath);
            }

            _ = Assert.Throws<ArgumentNullException>(testAction);
        }

        [Fact]
        public async Task AssembleAtAsync_FilePathNull_ThrowsArgumentNullException()
        {
            var fileAssembler = CreateDefaultFileAssembler();
            var workspaceId = CreateDefaultWorkspaceId();
            var filePath = (string)null;

            async Task testAction()
            {
                await fileAssembler.AssembleAtAsync(workspaceId, filePath);
            }

            _ = await Assert.ThrowsAsync<ArgumentNullException>(testAction);
        }

        [Theory]
        [ClassData(typeof(AssembleAtTestData))]
        public void AssembleAt_CorrectParameters_CreatesFileWithExpectedContent(byte[][] filePartsBytes, byte[] expectedFileBytes)
        {
            var fileAssembler = CreateDefaultFileAssembler();

            var workspaceId = fileAssembler.CreateWorkspace();
            for (var i = 0; i < filePartsBytes.Length; i++)
            {
                var filePartBytes = filePartsBytes[i];
                using var filePartData = new MemoryStream(filePartBytes, writable: false);
                var filePartNumber = i + 1;
                fileAssembler.PutPart(workspaceId, filePartData, filePartNumber);
            }

            var filePath = GetFilePath(workspaceId);
            fileAssembler.AssembleAt(workspaceId, filePath);
            var fileBytes = File.ReadAllBytes(filePath);

            Assert.Equal(expectedFileBytes, fileBytes);
        }

        [Theory]
        [ClassData(typeof(AssembleAtTestData))]
        public async Task AssembleAtAsync_CorrectParameters_CreatesFileWithExpectedContent(byte[][] filePartsBytes, byte[] expectedFileBytes)
        {
            var fileAssembler = CreateDefaultFileAssembler();

            var workspaceId = fileAssembler.CreateWorkspace();
            for (var i = 0; i < filePartsBytes.Length; i++)
            {
                var filePartBytes = filePartsBytes[i];
                using var filePartData = new MemoryStream(filePartBytes, writable: false);
                var filePartNumber = i + 1;
                await fileAssembler.PutPartAsync(workspaceId, filePartData, filePartNumber);
            }

            var filePath = GetFilePath(workspaceId);
            await fileAssembler.AssembleAtAsync(workspaceId, filePath);
            var fileBytes = File.ReadAllBytes(filePath);

            Assert.Equal(expectedFileBytes, fileBytes);
        }

        private class AssembleAtTestData : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                yield return new object[] { Array.Empty<byte[]>(), Array.Empty<byte>() };
                yield return new object[] { new byte[][] { Array.Empty<byte>() }, Array.Empty<byte>() };
                yield return new object[] { new byte[][] { Array.Empty<byte>(), Array.Empty<byte>() }, Array.Empty<byte>() };
                yield return new object[] { new byte[][] { new byte[] { 1 }, new byte[] { 2 } }, new byte[] { 1, 2 } };
                yield return new object[] { new byte[][] { new byte[] { 2 }, new byte[] { 1 } }, new byte[] { 2, 1 } };
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }
    }
}