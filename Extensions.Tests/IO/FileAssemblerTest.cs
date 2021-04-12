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
        public void Ctor_WorkPathNull_ThrowsArgumentNullException()
        {
            var workPath = (string)null;

            void testAction()
            {
                _ = new FileAssembler(workPath);
            }

            _ = Assert.Throws<ArgumentNullException>(testAction);
        }

        [Fact]
        public void CreateWorkspace_NoPrameters_CreatesDirectory()
        {
            var fileAssembler = CreateDefaultFileAssembler();

            var workspaceId = fileAssembler.CreateWorkspace();
            var workspacePath = GetWorkspaceDirectoryPath(workspaceId);
            var workspaceExists = Directory.Exists(workspacePath);

            Assert.True(workspaceExists);
        }

        [Fact]
        public void CheckWorkspaceExists_WorkspaceNotExists_ReturnsFalse()
        {
            var fileAssembler = CreateDefaultFileAssembler();
            var workspaceId = CreateDefaultWorkspaceId();

            var workspaceExists = fileAssembler.CheckWorkspaceExists(workspaceId);

            Assert.False(workspaceExists);
        }

        [Fact]
        public void CheckWorkspaceExists_WorkspaceExists_ReturnsTrue()
        {
            var fileAssembler = CreateDefaultFileAssembler();

            var workspaceId = fileAssembler.CreateWorkspace();
            var workspaceExists = fileAssembler.CheckWorkspaceExists(workspaceId);

            Assert.True(workspaceExists);
        }

        [Fact]
        public void DeleteWorkspace_WorkspaceNotExists_ThrowsDirectoryNotFoundException()
        {
            var fileAssembler = CreateDefaultFileAssembler();
            var workspaceId = CreateDefaultWorkspaceId();

            void testAction()
            {
                fileAssembler.DeleteWorkspace(workspaceId);
            }

            _ = Assert.Throws<DirectoryNotFoundException>(testAction);
        }

        [Fact]
        public void DeleteWorkspace_WorkspaceExists_DeletesDirectory()
        {
            var fileAssembler = CreateDefaultFileAssembler();

            var workspaceId = fileAssembler.CreateWorkspace();
            var workspacePath = GetWorkspaceDirectoryPath(workspaceId);
            fileAssembler.DeleteWorkspace(workspaceId);
            var workspaceNotExists = !Directory.Exists(workspacePath);

            Assert.True(workspaceNotExists);
        }

        private static Guid CreateDefaultWorkspaceId()
        {
            return Guid.Empty;
        }

        private static Stream CreateDefaultPartData()
        {
            return new MemoryStream(Array.Empty<byte>(), writable: false);
        }

        private static int CreateDefaultPartNumber()
        {
            return FileAssembler.PartMinNumber;
        }

        private FileAssembler CreateDefaultFileAssembler()
        {
            return new FileAssembler(this.directoryFixture.Path);
        }

        private string GetWorkspaceDirectoryPath(Guid workspaceId)
        {
            return Path.Combine(this.directoryFixture.Path, $"{workspaceId:N}");
        }

        private string GetPartFilePath(Guid workspaceId, int filePartNumber)
        {
            return Path.Combine(this.directoryFixture.Path, $"{workspaceId:N}", $"{filePartNumber:D5}{FileAssembler.PartFileNameExt}");
        }

        private string GetFilePath(Guid workspaceId)
        {
            const string testFileName = "testfile";
            return Path.Combine(this.directoryFixture.Path, $"{workspaceId:N}", testFileName);
        }
    }
}