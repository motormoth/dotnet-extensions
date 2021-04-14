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
        public void AssembleChunks_DestinationNull_ThrowsArgumentNullException()
        {
            var fileAssembler = GetFileAssembler();
            var workspaceId = CreateDefaultWorkspaceId();
            var destination = (Stream)null;

            void testAction()
            {
                _ = fileAssembler.AssembleChunks(workspaceId, destination);
            }

            _ = Assert.Throws<ArgumentNullException>(testAction);
        }

        [Fact]
        public async Task AssembleChunksAsync_DestinationNull_ThrowsArgumentNullException()
        {
            var fileAssembler = GetFileAssembler();
            var workspaceId = CreateDefaultWorkspaceId();
            var destination = (Stream)null;

            async Task testAction()
            {
                _ = await fileAssembler.AssembleChunksAsync(workspaceId, destination);
            }

            _ = await Assert.ThrowsAsync<ArgumentNullException>(testAction);
        }

        [Theory]
        [ClassData(typeof(AssembleChunksTestData))]
        public void AssembleChunks_Default_CreatesFileWithExpectedContent(byte[][] chunksBytes, byte[] expectedBytes)
        {
            var fileAssembler = GetFileAssembler();
            using var destination = CreateDefaultAssembleDestination();

            var workspaceId = fileAssembler.CreateWorkspace();
            for (var i = 0; i < chunksBytes.Length; i++)
            {
                var chunkBytes = chunksBytes[i];
                using var chunkSource = new MemoryStream(chunkBytes, writable: false);
                var chunkIndex = (ushort)i;
                _ = fileAssembler.PutChunk(workspaceId, chunkSource, chunkIndex);
            }

            _ = fileAssembler.AssembleChunks(workspaceId, destination);
            var bytes = destination.ToArray();

            Assert.Equal(expectedBytes, bytes);
        }

        [Theory]
        [ClassData(typeof(AssembleChunksTestData))]
        public async Task AssembleChunksAsync_Default_CreatesFileWithExpectedContent(byte[][] chunksBytes, byte[] expectedBytes)
        {
            var fileAssembler = GetFileAssembler();
            using var destination = CreateDefaultAssembleDestination();

            var workspaceId = fileAssembler.CreateWorkspace();
            for (var i = 0; i < chunksBytes.Length; i++)
            {
                var chunkBytes = chunksBytes[i];
                using var chunkSource = new MemoryStream(chunkBytes, writable: false);
                var chunkIndex = (ushort)i;
                _ = await fileAssembler.PutChunkAsync(workspaceId, chunkSource, chunkIndex);
            }

            _ = await fileAssembler.AssembleChunksAsync(workspaceId, destination);
            var bytes = destination.ToArray();

            Assert.Equal(expectedBytes, bytes);
        }

        private class AssembleChunksTestData : IEnumerable<object[]>
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