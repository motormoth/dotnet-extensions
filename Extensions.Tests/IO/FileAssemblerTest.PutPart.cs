// Licensed under the MIT License. See LICENSE file for full license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

using Xunit;

using Motormoth.Extensions.IO;

namespace Motormoth.Extensions.Tests.IO
{
    partial class FileAssemblerTest
    {
        [Fact]
        public void PutPart_PartDataNull_ThrowsArgumentNullException()
        {
            var fileAssembler = CreateDefaultFileAssembler();
            var workspaceId = CreateDefaultWorkspaceId();
            var partData = (Stream)null;
            var partNumber = CreateDefaultPartNumber();

            void testAction()
            {
                fileAssembler.PutPart(workspaceId, partData, partNumber);
            }

            _ = Assert.Throws<ArgumentNullException>(testAction);
        }

        [Fact]
        public async Task PutPartAsync_PartDataNull_ThrowsArgumentNullException()
        {
            var fileAssembler = CreateDefaultFileAssembler();
            var workspaceId = CreateDefaultWorkspaceId();
            var partData = (Stream)null;
            var partNumber = CreateDefaultPartNumber();

            async Task testAction()
            {
                await fileAssembler.PutPartAsync(workspaceId, partData, partNumber);
            }

            _ = await Assert.ThrowsAsync<ArgumentNullException>(testAction);
        }

        [Theory]
        [InlineData(FileAssembler.PartMinNumber - 1)]
        [InlineData(FileAssembler.PartMaxNumber + 1)]
        public void PutPart_PartNumberInvalid_ThrowsArgumentOutOfRangeException(int partNumber)
        {
            var fileAssembler = CreateDefaultFileAssembler();
            var workspaceId = CreateDefaultWorkspaceId();
            using var partData = CreateDefaultPartData();

            void testAction()
            {
                fileAssembler.PutPart(workspaceId, partData, partNumber);
            }

            _ = Assert.Throws<ArgumentOutOfRangeException>(testAction);
        }

        [Theory]
        [InlineData(FileAssembler.PartMinNumber - 1)]
        [InlineData(FileAssembler.PartMaxNumber + 1)]
        public async Task PutPartAsync_PartNumberInvalid_ThrowsArgumentOutOfRangeException(int partNumber)
        {
            var fileAssembler = CreateDefaultFileAssembler();
            var workspaceId = CreateDefaultWorkspaceId();
            using var partData = CreateDefaultPartData();

            async Task testAction()
            {
                await fileAssembler.PutPartAsync(workspaceId, partData, partNumber);
            }

            _ = await Assert.ThrowsAsync<ArgumentOutOfRangeException>(testAction);
        }

        [Theory]
        [ClassData(typeof(PutPartTestData))]
        public void PutPart_CorrectParameters_CreatesFileWithExpectedContent(byte[] partBytes)
        {
            var fileAssembler = CreateDefaultFileAssembler();
            using var partData = new MemoryStream(partBytes, writable: false);
            var partNumber = CreateDefaultPartNumber();

            var workspaceId = fileAssembler.CreateWorkspace();
            fileAssembler.PutPart(workspaceId, partData, partNumber);
            var partFilePath = GetPartFilePath(workspaceId, partNumber);
            var partFileBytes = File.ReadAllBytes(partFilePath);

            Assert.Equal(partBytes, partFileBytes);
        }

        [Theory]
        [ClassData(typeof(PutPartTestData))]
        public async Task PutPartAsync_CorrectParameters_CreatesFileWithExpectedContent(byte[] partBytes)
        {
            var fileAssembler = CreateDefaultFileAssembler();
            using var partData = new MemoryStream(partBytes, writable: false);
            var partNumber = CreateDefaultPartNumber();

            var workspaceId = fileAssembler.CreateWorkspace();
            await fileAssembler.PutPartAsync(workspaceId, partData, partNumber);
            var partFilePath = GetPartFilePath(workspaceId, partNumber);
            var partFileBytes = File.ReadAllBytes(partFilePath);

            Assert.Equal(partBytes, partFileBytes);
        }

        private class PutPartTestData : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                yield return new object[] { Array.Empty<byte>()};
                yield return new object[] { new byte[] { 1 }};
                yield return new object[] { new byte[] { 1, 2 }};
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }
    }
}