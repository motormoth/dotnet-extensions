// Licensed under the MIT License. See LICENSE file for full license information.

using System;
using System.IO;
using System.IO.Pipelines;
using System.Threading;
using System.Threading.Tasks;

using Motormoth.Extensions.IO.Pipelines;

using Xunit;

namespace Motormoth.Extensions.Tests.IO.Pipelines
{
    public sealed class PipeReaderCopyDataExtensionsTest
    {
        [Fact]
        public async void CopyDataAsync_NullSource_ThrowsArgumentNullException()
        {
            var source = CreateNullPipeReader();
            var destination = CreateFakePipeWriter();

            async Task copyAction()
            {
                _ = await PipeReaderCopyDataExtensions.CopyDataAsync(source, destination, 1, CancellationToken.None);
            }

            _ = await Assert.ThrowsAsync<ArgumentNullException>(copyAction);
        }

        [Fact]
        public async void CopyDataAsync_NullDestination_ThrowsArgumentNullException()
        {
            var source = CreateFakePipeReader();
            var destination = CreateNullPipeWriter();

            async Task copyAction()
            {
                _ = await PipeReaderCopyDataExtensions.CopyDataAsync(source, destination, 1, CancellationToken.None);
            }

            _ = await Assert.ThrowsAsync<ArgumentNullException>(copyAction);
        }

        [Theory]
        [InlineData(new byte[0], 0, 0, new byte[0])]
        [InlineData(new byte[0], +1, 0, new byte[0])]
        [InlineData(new byte[0], -1, 0, new byte[0])]
        [InlineData(new byte[1] { 1 }, 0, 0, new byte[0])]
        [InlineData(new byte[1] { 1 }, +1, 1, new byte[1] { 1 })]
        [InlineData(new byte[1] { 1 }, +2, 1, new byte[1] { 1 })]
        [InlineData(new byte[1] { 1 }, -1, 1, new byte[1] { 1 })]
        [InlineData(new byte[2] { 1, 2 }, 0, 0, new byte[0])]
        [InlineData(new byte[2] { 1, 2 }, +1, 1, new byte[1] { 1 })]
        [InlineData(new byte[2] { 1, 2 }, +2, 2, new byte[2] { 1, 2 })]
        [InlineData(new byte[2] { 1, 2 }, -1, 2, new byte[2] { 1, 2 })]
        public async void CopyDataAsync_CorrectArguments_CopiesData(byte[] sourceArray, long copyLimit,
            long expectedCopyCount, byte[] expectedDestinationArray)
        {
            using var sourceStream = new MemoryStream(sourceArray, false);
            using var destinationStream = new MemoryStream();
            var source = PipeReader.Create(sourceStream);
            var destination = PipeWriter.Create(destinationStream);

            var copyCount = await PipeReaderCopyDataExtensions.CopyDataAsync(source, destination, copyLimit, cancellationToken: CancellationToken.None);
            var destinationArray = destinationStream.ToArray();

            Assert.Equal(expectedCopyCount, copyCount);
            Assert.Equal(expectedDestinationArray, destinationArray);
        }

        private static PipeReader CreateNullPipeReader()
        {
            return null;
        }

        private static PipeWriter CreateNullPipeWriter()
        {
            return null;
        }

        private static PipeReader CreateFakePipeReader()
        {
            return PipeReader.Create(Stream.Null);
        }

        private static PipeWriter CreateFakePipeWriter()
        {
            return PipeWriter.Create(Stream.Null);
        }
    }
}