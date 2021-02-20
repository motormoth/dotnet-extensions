// Licensed under the MIT License. See LICENSE file for full license information.

namespace Motormoth.Extensions.IO.Pipelines
{
    public partial class PipeReaderCopyDataExtensions
    {
        private const string Error_ReadCanceledOnReader = "Read was canceled on underlying PipeReader.";

        private const string Error_FlushCanceledOnWriter = "Flush was canceled on underlying PipeWriter.";
    }
}