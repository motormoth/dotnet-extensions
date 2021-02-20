// Licensed under the MIT License. See LICENSE file for full license information.

using System.IO;

using Xunit;

namespace Motormoth.Extensions.Tests.IO
{
    partial class FileAssemblerTest
    {
        [Fact]
        public void DeleteWorkspace_WorkSpaceExsits_ReturnsTrue()
        {
            var fileAssembler = CreateFileAssembler();

            var workspaceId = fileAssembler.CreateWorkspace();
            var workspaceFound = fileAssembler.DeleteWorkspace(workspaceId);

            Assert.True(workspaceFound);
        }

        [Fact]
        public void DeleteWorkspace_WorkSpaceNotExsits_ReturnsFalse()
        {
            var fileAssembler = CreateFileAssembler();
            var workspaceId = CreateDefaultWorkspaceId();

            var workspaceFound = fileAssembler.DeleteWorkspace(workspaceId);

            Assert.False(workspaceFound);
        }

        [Fact]
        public void DeleteWorkspace_DeletesDirectory()
        {
            var fileAssembler = CreateFileAssembler();

            var workspaceId = fileAssembler.CreateWorkspace();
            var workspacePath = GetWorkspacePath(workspaceId);
            _ = fileAssembler.DeleteWorkspace(workspaceId);
            var workspaceNotExists = !Directory.Exists(workspacePath);

            Assert.True(workspaceNotExists);
        }
    }
}