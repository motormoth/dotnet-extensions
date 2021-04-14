// Licensed under the MIT License. See LICENSE file for full license information.

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Motormoth.Extensions.IO
{
    /// <summary>
    /// Provides methods for assembling files from chunks.
    /// </summary>
    public sealed class DataAssembler
    {
        /// <summary>
        /// The chunk file name extension.
        /// </summary>
        public const string ChunkFileNameExt = ".chunk";

        /// <summary>
        /// Initializes a new instance of the <see cref="DataAssembler"/> class.
        /// </summary>
        /// <param name="workPath">
        /// The path to a work directory in which the data assembler will store
        /// it's data.
        /// </param>
        public DataAssembler(string workPath)
        {
            if (workPath is null)
            {
                throw new ArgumentNullException(nameof(workPath));
            }

            WorkPath = Path.GetFullPath(workPath);
        }

        /// <summary>
        /// Gets the path to a work directory of the data assembler.
        /// </summary>
        public string WorkPath { get; }

        /// <summary>
        /// Creates a workspace.
        /// </summary>
        /// <returns>The unique identifier of a workspace.</returns>
        public Guid CreateWorkspace()
        {
            var workspaceId = Guid.NewGuid();
            var workspacePath = GetWorkspacePath(WorkPath, workspaceId);
            _ = Directory.CreateDirectory(workspacePath);
            return workspaceId;
        }

        /// <summary>
        /// Deletes a workspace.
        /// </summary>
        /// <param name="id">The id of a workspace to delete.</param>
        /// <returns>
        /// The value that indicates whether the workspace was found or not.
        /// </returns>
        public bool DeleteWorkspace(Guid id)
        {
            var workspacePath = GetWorkspacePath(WorkPath, id);
            if (Directory.Exists(workspacePath))
            {
                try
                {
                    Directory.Delete(workspacePath, recursive: true);
                    return true;
                }
                catch (DirectoryNotFoundException)
                {
                    return false;
                }
            }

            return false;
        }

        /// <summary>
        /// Asynchronously puts a chunk into a workspace.
        /// </summary>
        /// <remarks>
        /// This method will replace a chunk with same index, if it already exists.
        /// </remarks>
        /// <param name="workspaceId">
        /// The unique identifier of a workspace to which chunk will be put.
        /// </param>
        /// <param name="source">The stream from which chunk data will be copied.</param>
        /// <param name="index">The index of a chunk.</param>
        /// <param name="cancellationToken">
        /// The token to monitor for cancellation requests. The default value is
        /// <see cref="CancellationToken.None"/>.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous operation with the result
        /// value that indicates whether the workspace was found or not.
        /// </returns>
        public async Task<bool> PutChunkAsync(Guid workspaceId,
            Stream source, ushort index,
            CancellationToken cancellationToken = default)
        {
            CheckPutChunkParameters(source);

            var workspacePath = GetWorkspacePath(WorkPath, workspaceId);
            if (Directory.Exists(workspacePath))
            {
                try
                {
                    var chunkPath = GetChunkPath(workspacePath, index);
                    await using var chunkStream = File.Open(chunkPath,
                        FileMode.Create, FileAccess.Write);
                    await source.CopyToAsync(chunkStream, cancellationToken);
                    return true;
                }
                catch (DirectoryNotFoundException)
                {
                    return false;
                }
            }

            return false;
        }

        /// <summary>
        /// Puts a chunk into a workspace.
        /// </summary>
        /// <remarks>
        /// This method will replace a chunk with same index, if it already exists.
        /// </remarks>
        /// <param name="workspaceId">
        /// The unique identifier of a workspace to which chunk will be put.
        /// </param>
        /// <param name="source">The stream from which data will be copied.</param>
        /// <param name="index">The index of a chunk.</param>
        /// <returns>
        /// The value that indicates whether the workspace was found or not.
        /// </returns>
        public bool PutChunk(Guid workspaceId,
            Stream source, ushort index)
        {
            CheckPutChunkParameters(source);

            var workspacePath = GetWorkspacePath(WorkPath, workspaceId);
            if (Directory.Exists(workspacePath))
            {
                try
                {
                    var chunkPath = GetChunkPath(workspacePath, index);
                    using var chunkStream = File.Open(chunkPath,
                        FileMode.Create, FileAccess.Write);
                    source.CopyTo(chunkStream);
                    return true;
                }
                catch (DirectoryNotFoundException)
                {
                    return false;
                }
            }

            return false;
        }

        /// <summary>
        /// Asynchronously assembles chunks from a workspace.
        /// </summary>
        /// <param name="workspaceId">
        /// The unique identifier of a workspace from which chunks will be assembled.
        /// </param>
        /// <param name="destination">The stream to which data will be copied.</param>
        /// <param name="deleteWorkspace">
        /// The value that indicates whether the workspace must be deleted after
        /// successfull assembly. The default value is true.
        /// </param>
        /// <param name="cancellationToken">
        /// The token to monitor for cancellation requests. The default value is
        /// <see cref="CancellationToken.None"/>.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous operation with the result
        /// value that indicates whether the workspace was found or not.
        /// </returns>
        public async Task<bool> AssembleChunksAsync(Guid workspaceId,
            Stream destination, bool deleteWorkspace = true,
            CancellationToken cancellationToken = default)
        {
            CheckAssembleChunksParameters(destination);

            var workspacePath = GetWorkspacePath(WorkPath, workspaceId);
            if (Directory.Exists(workspacePath))
            {
                try
                {
                    var presentChunkPaths = GetPresentChunkPaths(workspacePath);
                    foreach (var presentChunkPath in presentChunkPaths)
                    {
                        await using var presentChunkStream = File.Open(presentChunkPath,
                            FileMode.Open, FileAccess.Read);
                        await presentChunkStream.CopyToAsync(destination, cancellationToken);
                    }

                    if (deleteWorkspace)
                    {
                        try
                        {
                            Directory.Delete(workspacePath, recursive: true);
                        }
                        catch (DirectoryNotFoundException)
                        {
                        }
                    }

                    return true;
                }
                catch (DirectoryNotFoundException)
                {
                    return false;
                }
            }

            return false;
        }

        /// <summary>
        /// Assembles chunks from a workspace.
        /// </summary>
        /// <param name="workspaceId">
        /// The unique identifier of a workspace from which chunks will be assembled.
        /// </param>
        /// <param name="destination">The stream to which data will be copied.</param>
        /// <param name="deleteWorkspace">
        /// The value that indicates whether the workspace must be deleted after
        /// successfull assembly. The default value is true.
        /// </param>
        /// <returns>
        /// The value that indicates whether the workspace was found or not.
        /// </returns>
        public bool AssembleChunks(Guid workspaceId,
            Stream destination, bool deleteWorkspace = true)
        {
            CheckAssembleChunksParameters(destination);

            var workspacePath = GetWorkspacePath(WorkPath, workspaceId);
            if (Directory.Exists(workspacePath))
            {
                try
                {
                    var presentChunkPaths = GetPresentChunkPaths(workspacePath);
                    foreach (var presentChunkPath in presentChunkPaths)
                    {
                        using var presentChunkStream = File.Open(presentChunkPath,
                            FileMode.Open, FileAccess.Read);
                        presentChunkStream.CopyTo(destination);
                    }

                    if (deleteWorkspace)
                    {
                        try
                        {
                            Directory.Delete(workspacePath, recursive: true);
                        }
                        catch (DirectoryNotFoundException)
                        {
                        }
                    }

                    return true;
                }
                catch (DirectoryNotFoundException)
                {
                    return false;
                }
            }

            return false;
        }

        private static void CheckPutChunkParameters(Stream source)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }
        }

        private static void CheckAssembleChunksParameters(Stream destination)
        {
            if (destination is null)
            {
                throw new ArgumentNullException(nameof(destination));
            }
        }

        private static string[] GetPresentChunkPaths(string workspacePath)
        {
            var presentChunkPaths = Directory.GetFiles(workspacePath, $"*{ChunkFileNameExt}");
            Array.Sort(presentChunkPaths, StringComparer.InvariantCulture);
            return presentChunkPaths;
        }

        private static string GetChunkPath(string workspacePath, ushort chunkIndex)
        {
            return Path.GetFullPath(GetChunkName(chunkIndex), workspacePath);
        }

        private static string GetWorkspacePath(string workPath, Guid workspaceId)
        {
            return Path.GetFullPath(GetWorkspaceName(workspaceId), workPath);
        }

        private static string GetChunkName(ushort chunkIndex)
        {
            return $"{chunkIndex:D6}{ChunkFileNameExt}";
        }

        private static string GetWorkspaceName(Guid workspaceId)
        {
            return $"{workspaceId:N}";
        }
    }
}