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
        /// The minimum possible part number.
        /// </summary>
        public const int PartMinNumber = 1;

        /// <summary>
        /// The maximum possible part number.
        /// </summary>
        public const int PartMaxNumber = 20000;

        /// <summary>
        /// The part file name extension.
        /// </summary>
        public const string PartFileNameExt = ".part";

        /// <summary>
        /// Initializes a new instance of the <see cref="FileAssembler"/> class
        /// without output directory.
        /// </summary>
        /// <param name="workPath">
        /// The path to a directory in which the file assembler will store
        /// intermediate data.
        /// </param>
        public FileAssembler(string workPath)
            : this(workPath, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileAssembler"/> class.
        /// </summary>
        /// <param name="workPath">
        /// The path to a work directory in which the file assembler will store
        /// intermediate data.
        /// </param>
        /// <param name="outputPath">
        /// The path to an output directory in which the file assembler will
        /// store asembled files.
        /// </param>
        public FileAssembler(string workPath, string outputPath)
        {
            if (workPath is null)
            {
                throw new ArgumentNullException(nameof(workPath));
            }

            WorkPath = workPath;
            OutputPath = outputPath;
        }

        /// <summary>
        /// Gets the path to a work directory of the file assembler.
        /// </summary>
        public string WorkPath { get; }

        /// <summary>
        /// Gets or sets the path to an output directory of the file assembler.
        /// </summary>
        public string OutputPath { get; set; }

        /// <summary>
        /// Creates a workspace.
        /// </summary>
        /// <returns>The unique identifier of a workspace.</returns>
        public Guid CreateWorkspace()
        {
            var workspaceId = Guid.NewGuid();
            var workspacePath = GetWorkspacePath(workspaceId);
            _ = Directory.CreateDirectory(workspacePath);
            return workspaceId;
        }

        /// <summary>
        /// Checks if a workspace exists.
        /// </summary>
        /// <param name="workspaceId">The id of a workspace to check.</param>
        /// <returns>
        /// The value that indicates whether a workspace exists or not.
        /// </returns>
        public bool CheckWorkspaceExists(Guid workspaceId)
        {
            var workspacePath = GetWorkspacePath(workspaceId);
            return Directory.Exists(workspacePath);
        }

        /// <summary>
        /// Deletes a workspace.
        /// </summary>
        /// <param name="workspaceId">The id of a workspace to delete.</param>
        public void DeleteWorkspace(Guid workspaceId)
        {
            var workspacePath = GetWorkspacePath(workspaceId);
            Directory.Delete(workspacePath, recursive: true);
        }

        /// <summary>
        /// Puts a part into a workspace.
        /// </summary>
        /// <remarks>This method will replace a part, if it already exists.</remarks>
        /// <param name="workspaceId">
        /// The id of a workspace to which part will be added.
        /// </param>
        /// <param name="partData">
        /// The stream from which part data will be copied.
        /// </param>
        /// <param name="partNumber">
        /// The index number of a part. The value must be between <see
        /// cref="PartMinNumber"/> and <see cref="PartMaxNumber"/>.
        /// </param>
        public void PutPart(Guid workspaceId,
            Stream partData, int partNumber)
        {
            CheckPutPartParameters(partData, partNumber);

            var partPath = GetPartPath(workspaceId, partNumber);
            using var partStream = File.Open(partPath,
                FileMode.Create, FileAccess.Write);
            partData.CopyTo(partStream);
        }

        /// <summary>
        /// Asynchronously puts a part into a workspace.
        /// </summary>
        /// <remarks>This method will replace a part, if it already exists.</remarks>
        /// <param name="workspaceId">
        /// The id of a workspace to which part will be added.
        /// </param>
        /// <param name="partData">
        /// The stream from which part data will be copied.
        /// </param>
        /// <param name="partNumber">
        /// The index number of a part. The value must be between <see
        /// cref="PartMinNumber"/> and <see cref="PartMaxNumber"/>.
        /// </param>
        /// <param name="cancellationToken">
        /// The token to monitor for cancellation requests. The default value is
        /// <see cref="CancellationToken.None"/>.
        /// </param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task PutPartAsync(Guid workspaceId,
            Stream partData, int partNumber,
            CancellationToken cancellationToken = default)
        {
            CheckPutPartParameters(partData, partNumber);

            var partPath = GetPartPath(workspaceId, partNumber);
            await using var partStream = File.Open(partPath,
                FileMode.Create, FileAccess.Write);
            await partData.CopyToAsync(partStream, cancellationToken);
        }

        /// <summary>
        /// Assembles a file from a workspace.
        /// </summary>
        /// <remarks>
        /// If the <see cref="OutputPath"/> property is not set, this method
        /// will throw <see cref="InvalidOperationException"/>.
        /// </remarks>
        /// <param name="workspaceId">
        /// The id of a workspace from which a file will be assembled.
        /// </param>
        public void Assemble(Guid workspaceId)
        {
            if (OutputPath is null)
            {
                throw new InvalidOperationException();
            }

            var filePath = GetFilePath(workspaceId);
            AssembleAt(workspaceId, filePath);
        }

        /// <summary>
        /// Asynchronously assembles a file from a workspace.
        /// </summary>
        /// <remarks>
        /// If <see cref="OutputPath"/> property is not set, this method will
        /// throw <see cref="InvalidOperationException"/>.
        /// </remarks>
        /// <param name="workspaceId">
        /// The id of a workspace from which a file will be assembled.
        /// </param>
        /// <param name="cancellationToken">
        /// The token to monitor for cancellation requests. The default value is
        /// <see cref="CancellationToken.None"/>.
        /// </param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task AssembleAsync(Guid workspaceId,
            CancellationToken cancellationToken = default)
        {
            if (OutputPath is null)
            {
                throw new InvalidOperationException();
            }

            var filePath = GetFilePath(workspaceId);
            await AssembleAtAsync(workspaceId, filePath, cancellationToken);
        }

        /// <summary>
        /// Assembles a file from a workspace.
        /// </summary>
        /// <param name="workspaceId">
        /// The id of a workspace from which a file will be assembled.
        /// </param>
        /// <param name="filePath">
        /// The path at which an assembled file will be created.
        /// </param>
        public void AssembleAt(Guid workspaceId,
            string filePath)
        {
            CheckAssembleAtParameters(filePath);

            var partPaths = GetPartPaths(workspaceId);
            using var fileStream = File.Open(filePath,
                FileMode.OpenOrCreate, FileAccess.Write);
            foreach (var partPath in partPaths)
            {
                using var partStream = File.Open(partPath,
                    FileMode.Open, FileAccess.Read);
                partStream.CopyTo(fileStream);
            }
        }

        /// <summary>
        /// Asynchronously assembles a file from a workspace.
        /// </summary>
        /// <param name="workspaceId">
        /// The id of a workspace from which a file will be assembled.
        /// </param>
        /// <param name="filePath">
        /// The path at which an assembled file will be created.
        /// </param>
        /// <param name="cancellationToken">
        /// The token to monitor for cancellation requests. The default value is
        /// <see cref="CancellationToken.None"/>.
        /// </param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task AssembleAtAsync(Guid workspaceId,
            string filePath,
            CancellationToken cancellationToken = default)
        {
            CheckAssembleAtParameters(filePath);

            var partPaths = GetPartPaths(workspaceId);
            await using var fileStream = File.Open(filePath,
                FileMode.OpenOrCreate, FileAccess.Write);
            foreach (var partPath in partPaths)
            {
                await using var partStream = File.Open(partPath,
                    FileMode.Open, FileAccess.Read);
                await partStream.CopyToAsync(fileStream, cancellationToken);
            }
        }

        private static void CheckPutPartParameters(Stream partData, int partNumber)
        {
            if (partNumber < PartMinNumber || partNumber > PartMaxNumber)
            {
                throw new ArgumentOutOfRangeException(nameof(partNumber));
            }

            if (partData is null)
            {
                throw new ArgumentNullException(nameof(partData));
            }
        }

        private static void CheckAssembleAtParameters(string filePath)
        {
            if (filePath is null)
            {
                throw new ArgumentNullException(nameof(filePath));
            }
        }

        private string[] GetPartPaths(Guid workspaceId)
        {
            var workspacePath = GetWorkspacePath(workspaceId);
            var partPaths = Directory.GetFiles(workspacePath, $"*{PartFileNameExt}");
            Array.Sort(partPaths, StringComparer.InvariantCulture);
            return partPaths;
        }

        private string GetPartPath(Guid workspaceId, int partNumber)
        {
            var workspacePath = GetWorkspacePath(workspaceId);
            return Path.Join(workspacePath, $"{partNumber:D5}{PartFileNameExt}");
        }

        private string GetWorkspacePath(Guid workspaceId)
        {
            return Path.Join(WorkPath, $"{workspaceId:N}");
        }

        private string GetFilePath(Guid workspaceId)
        {
            return Path.Join(OutputPath, $"{workspaceId:N}");
        }
    }
}