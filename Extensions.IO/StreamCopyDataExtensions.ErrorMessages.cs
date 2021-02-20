// Licensed under the MIT License. See LICENSE file for full license information.

namespace Motormoth.Extensions.IO
{
    partial class StreamCopyDataExtensions
    {
        private const string Error_NonReadableSourceStream = "The source stream does not support reading.";

        private const string Error_NonWritableDestinationStream = "The destination stream does not support writing.";

        private const string Error_InvalidBufferSize = "The buffer size is negative or zero.";
    }
}