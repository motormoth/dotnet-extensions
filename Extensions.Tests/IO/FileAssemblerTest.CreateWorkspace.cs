// Licensed under the MIT License. See LICENSE file for full license information.

using System.IO;

using Xunit;

namespace Motormoth.Extensions.Tests.IO
{
    partial class FileAssemblerTest
    {
        [Fact]
        public void CreateWorkspace_NoPrameters_CreatesDirectory()
        {
            var fileAssembler = CreateFileAssembler();

            var workspaceId = fileAssembler.CreateWorkspace();
            var workspacePath = GetWorkspacePath(workspaceId);
            var workspaceExists = Directory.Exists(workspacePath);

            Assert.True(workspaceExists);
        }
    }
}