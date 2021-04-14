// Licensed under the MIT License. See LICENSE file for full license information.

using System;
using System.IO;

using Motormoth.Extensions.IO;

using Xunit;

namespace Motormoth.Extensions.Tests.IO
{
    public sealed partial class DataAssemblerTest : IClassFixture<DirectoryFixture>
    {
        private readonly DirectoryFixture directoryFixture;

        public DataAssemblerTest(DirectoryFixture directoryFixture)
        {
            this.directoryFixture = directoryFixture;
        }

        [Fact]
        public void Ctor_WorkPathNull_ThrowsArgumentNullException()
        {
            var workPath = (string)null;

            void testAction()
            {
                _ = new DataAssembler(workPath);
            }

            _ = Assert.Throws<ArgumentNullException>(testAction);
        }

        [Fact]
        public void CreateWorkspace_Default_ReturnsValidGuid()
        {
            var fileAssembler = GetFileAssembler();

            var workspaceId = fileAssembler.CreateWorkspace();

            Assert.NotEqual(Guid.Empty, workspaceId);
        }

        [Fact]
        public void CreateWorkspace_Default_CreatesDirectory()
        {
            var fileAssembler = GetFileAssembler();

            var workspaceId = fileAssembler.CreateWorkspace();
            var workspacePath = GetWorkspaceDirectoryPath(workspaceId);
            var workspaceExists = Directory.Exists(workspacePath);

            Assert.True(workspaceExists);
        }

        [Fact]
        public void DeleteWorkspace_WorkspaceNotExists_ReturnsFalse()
        {
            var fileAssembler = GetFileAssembler();
            var workspaceId = CreateDefaultWorkspaceId();

            var workspaceFound = fileAssembler.DeleteWorkspace(workspaceId);

            Assert.False(workspaceFound);
        }

        [Fact]
        public void DeleteWorkspace_Default_ReturnsTrue()
        {
            var fileAssembler = GetFileAssembler();

            var workspaceId = fileAssembler.CreateWorkspace();
            var workspaceFound = fileAssembler.DeleteWorkspace(workspaceId);

            Assert.True(workspaceFound);
        }

        [Fact]
        public void DeleteWorkspace_Default_DeletesDirectory()
        {
            var fileAssembler = GetFileAssembler();

            var workspaceId = fileAssembler.CreateWorkspace();
            var workspacePath = GetWorkspaceDirectoryPath(workspaceId);
            _ = fileAssembler.DeleteWorkspace(workspaceId);
            var workspaceExists = Directory.Exists(workspacePath);

            Assert.False(workspaceExists);
        }

        private static Guid CreateDefaultWorkspaceId()
        {
            return Guid.Empty;
        }

        private static Stream CreateDefaultChunkSource()
        {
            return new MemoryStream(Array.Empty<byte>(), writable: false);
        }

        private static ushort CreateDefaultChunkIndex()
        {
            return 0;
        }

        private static MemoryStream CreateDefaultAssembleDestination()
        {
            return new MemoryStream();
        }

        private DataAssembler GetFileAssembler()
        {
            return new DataAssembler(this.directoryFixture.Path);
        }

        private string GetWorkspaceDirectoryPath(Guid workspaceId)
        {
            return Path.Combine(this.directoryFixture.Path, $"{workspaceId:N}");
        }

        private string GetChunkFilePath(Guid workspaceId, int chunkIndex)
        {
            return Path.Combine(this.directoryFixture.Path, $"{workspaceId:N}", $"{chunkIndex:D6}{DataAssembler.ChunkFileNameExt}");
        }
    }
}