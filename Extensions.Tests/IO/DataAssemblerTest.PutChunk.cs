// Licensed under the MIT License. See LICENSE file for full license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

using Xunit;

namespace Motormoth.Extensions.Tests.IO
{
    partial class DataAssemblerTest
    {
        [Fact]
        public void PutChunk_SourceNull_ThrowsArgumentNullException()
        {
            var fileAssembler = GetFileAssembler();
            var workspaceId = CreateDefaultWorkspaceId();
            var chunkData = (Stream)null;
            var chunkIndex = CreateDefaultChunkIndex();

            void testAction()
            {
                _ = fileAssembler.PutChunk(workspaceId, chunkData, chunkIndex);
            }

            _ = Assert.Throws<ArgumentNullException>(testAction);
        }

        [Fact]
        public async Task PutChunkAsync_SourceNull_ThrowsArgumentNullException()
        {
            var fileAssembler = GetFileAssembler();
            var workspaceId = CreateDefaultWorkspaceId();
            var chunkData = (Stream)null;
            var chunkIndex = CreateDefaultChunkIndex();

            async Task testAction()
            {
                _ = await fileAssembler.PutChunkAsync(workspaceId, chunkData, chunkIndex);
            }

            _ = await Assert.ThrowsAsync<ArgumentNullException>(testAction);
        }

        [Theory]
        [ClassData(typeof(PutChunkTestData))]
        public void PutChunk_Default_CreatesFileWithExpectedContent(byte[] chunkBytes)
        {
            var fileAssembler = GetFileAssembler();
            using var chunkData = new MemoryStream(chunkBytes, writable: false);
            var chunkIndex = CreateDefaultChunkIndex();

            var workspaceId = fileAssembler.CreateWorkspace();
            _ = fileAssembler.PutChunk(workspaceId, chunkData, chunkIndex);
            var chunkFilePath = GetChunkFilePath(workspaceId, chunkIndex);
            var chunkFileBytes = File.ReadAllBytes(chunkFilePath);

            Assert.Equal(chunkBytes, chunkFileBytes);
        }

        [Theory]
        [ClassData(typeof(PutChunkTestData))]
        public async Task PutChunkAsync_Default_CreatesFileWithExpectedContent(byte[] chunkBytes)
        {
            var fileAssembler = GetFileAssembler();
            using var chunkData = new MemoryStream(chunkBytes, writable: false);
            var chunkIndex = CreateDefaultChunkIndex();

            var workspaceId = fileAssembler.CreateWorkspace();
            _ = await fileAssembler.PutChunkAsync(workspaceId, chunkData, chunkIndex);
            var chunkFilePath = GetChunkFilePath(workspaceId, chunkIndex);
            var chunkFileBytes = File.ReadAllBytes(chunkFilePath);

            Assert.Equal(chunkBytes, chunkFileBytes);
        }

        private class PutChunkTestData : IEnumerable<object[]>
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