// Licensed under the MIT License. See LICENSE file for full license information.

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Motormoth.Extensions.IO
{
    /// <summary>
    /// Provides methods for assembling files from parts.
    /// </summary>
    public sealed class FileAssembler
    {
        /// <summary>
        /// The minimum possible file part number.
        /// </summary>
        public const int MinFilePartNumber = 1;

        /// <summary>
        /// The maximum possible file part number.
        /// </summary>
        public const int MaxFilePartNumber = 20000;

        /// <summary>
        /// The file part file extension.
        /// </summary>
        public const string FilePartFileExt = "part";

        private readonly DirectoryInfo rootDirectory;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileAssembler"/> class.
        /// </summary>
        /// <param name="rootPath">
        /// The path to a root directory in which the current file assembler will store intermediate data.
        /// </param>
        public FileAssembler(string rootPath)
        {
            if (rootPath is null)
            {
                throw new ArgumentNullException(nameof(rootPath));
            }

            this.rootDirectory = new DirectoryInfo(rootPath);
        }

        /// <summary>
        /// Creates a new workspace.
        /// </summary>
        /// <returns>The unique identifier of the workspace.</returns>
        public Guid CreateWorkspace()
        {
            var workspaceId = Guid.NewGuid();
            var workspacePath = GetWorkspacePath(this.rootDirectory.FullName, workspaceId);
            _ = Directory.CreateDirectory(workspacePath);
            return workspaceId;
        }

        /// <summary>
        /// Adds a file part to a workspace.
        /// </summary>
        /// <remarks>This method will replace a file part, if it already exists.</remarks>
        /// <param name="workspaceId">The id of the workspace to which file part will be added.</param>
        /// <param name="filePart">The stream from which data for the file part will be copied.</param>
        /// <param name="filePartNumber">
        /// The index number of the file part. The value must be between <see
        /// cref="MinFilePartNumber"/> and <see cref="MaxFilePartNumber"/>.
        /// </param>
        /// <returns>The value that indicates whether the workspace was found or not.</returns>
        public bool AddFilePart(Guid workspaceId, Stream filePart, int filePartNumber)
        {
            CheckAddFilePartParameters(filePart, filePartNumber);

            var workspacePath = GetWorkspacePath(this.rootDirectory.FullName, workspaceId);
            if (Directory.Exists(workspacePath))
            {
                var filePartPath = GetFilePartPath(workspacePath, filePartNumber);
                using var filePartFileStream = File.Open(filePartPath,
                    FileMode.Create, FileAccess.Write);
                filePart.CopyTo(filePartFileStream);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Asynchronously adds a file part to a workspace.
        /// </summary>
        /// <remarks>This method will replace a file part, if it already exists.</remarks>
        /// <param name="workspaceId">The id of the workspace to which file part will be added.</param>
        /// <param name="filePart">The stream from which data for the file part will be copied.</param>
        /// <param name="filePartNumber">
        /// The index number of the file part. The value must be between <see
        /// cref="MinFilePartNumber"/> and <see cref="MaxFilePartNumber"/>.
        /// </param>
        /// <param name="cancellationToken">
        /// The token to monitor for cancellation requests. The default value is <see cref="CancellationToken.None"/>.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous copy operation. The result value indicates
        /// whether the workspace was found or not.
        /// </returns>
        public async Task<bool> AddFilePartAsync(Guid workspaceId, Stream filePart, int filePartNumber,
            CancellationToken cancellationToken = default)
        {
            CheckAddFilePartParameters(filePart, filePartNumber);

            var workspacePath = GetWorkspacePath(this.rootDirectory.FullName, workspaceId);
            if (Directory.Exists(workspacePath))
            {
                var filePartPath = GetFilePartPath(workspacePath, filePartNumber);
                await using var filePartFileStream = File.Open(filePartPath,
                    FileMode.Create, FileAccess.Write);
                await filePart.CopyToAsync(filePartFileStream, cancellationToken);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Assembles a file from a workspace.
        /// </summary>
        /// <param name="workspaceId">The id of the workspace from which file will be assembled.</param>
        /// <param name="productPath">The path at which assembled file will be created.</param>
        /// <returns>The value that indicates whether the workspace was found or not.</returns>
        public bool AssembleFile(Guid workspaceId, string productPath)
        {
            CheckAssembleFileParameters(productPath);

            var workspacePath = GetWorkspacePath(this.rootDirectory.FullName, workspaceId);
            if (Directory.Exists(workspacePath))
            {
                var filePartPaths = Directory.GetFiles(workspacePath, $"*.{FilePartFileExt}");
                Array.Sort(filePartPaths, StringComparer.InvariantCultureIgnoreCase);
                using var productFileStream = File.Open(productPath,
                    FileMode.OpenOrCreate, FileAccess.Write);
                foreach (var filePartPath in filePartPaths)
                {
                    using var filePartFileStream = File.Open(filePartPath,
                        FileMode.Open, FileAccess.Read);
                    filePartFileStream.CopyTo(productFileStream);
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// Asynchronously assembles a file from a workspace.
        /// </summary>
        /// <param name="workspaceId">The id of the workspace from which file will be assembled.</param>
        /// <param name="productPath">The path at which assembled file will be created.</param>
        /// <param name="cancellationToken">
        /// The token to monitor for cancellation requests. The default value is <see cref="CancellationToken.None"/>.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous assemble operation. The result value indicates
        /// whether the workspace was found or not.
        /// </returns>
        public async Task<bool> AssembleFileAsync(Guid workspaceId, string productPath,
            CancellationToken cancellationToken = default)
        {
            CheckAssembleFileParameters(productPath);

            var workspacePath = GetWorkspacePath(this.rootDirectory.FullName, workspaceId);
            if (Directory.Exists(workspacePath))
            {
                var filePartPaths = Directory.GetFiles(workspacePath, $"*.{FilePartFileExt}");
                Array.Sort(filePartPaths, StringComparer.InvariantCultureIgnoreCase);
                await using var productFileStream = File.Open(productPath,
                    FileMode.OpenOrCreate, FileAccess.Write);
                foreach (var filePartPath in filePartPaths)
                {
                    await using var filePartFileStream = File.Open(filePartPath,
                        FileMode.Open, FileAccess.Read);
                    await filePartFileStream.CopyToAsync(productFileStream, cancellationToken);
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// Deletes a workspace.
        /// </summary>
        /// <param name="workspaceId">The id of the workspace to delete.</param>
        /// <returns>The value that indicates whether the workspace was found or not.</returns>
        public bool DeleteWorkspace(Guid workspaceId)
        {
            var workspacePath = GetWorkspacePath(this.rootDirectory.FullName, workspaceId);
            if (Directory.Exists(workspacePath))
            {
                try
                {
                    Directory.Delete(workspacePath, recursive: true);
                }
                catch (DirectoryNotFoundException)
                {
                    // ignore exception if directory was deleted before Delete called
                }

                return true;
            }

            return false;
        }

        private static void CheckAddFilePartParameters(Stream filePart, int filePartNumber)
        {
            if (filePartNumber < MinFilePartNumber || filePartNumber > MaxFilePartNumber)
            {
                throw new ArgumentOutOfRangeException(nameof(filePartNumber));
            }

            if (filePart is null)
            {
                throw new ArgumentNullException(nameof(filePart));
            }
        }

        private static void CheckAssembleFileParameters(string productPath)
        {
            if (productPath is null)
            {
                throw new ArgumentNullException(nameof(productPath));
            }
        }

        private static string GetWorkspacePath(string rootPath, Guid workspaceId)
        {
            return Path.Combine(rootPath, $"{workspaceId:N}");
        }

        private static string GetFilePartPath(string workspacePath, int filePartNumber)
        {
            return Path.Combine(workspacePath, $"{filePartNumber:D5}.{FilePartFileExt}");
        }
    }
}