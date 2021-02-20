// Licensed under the MIT License. See LICENSE file for full license information.

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

using Motormoth.Extensions.IO;

using Xunit;

namespace Motormoth.Extensions.Tests.IO
{
    public sealed class StreamCopyDataExtensionsTest
    {
        [Fact]
        public void CopyData_NullSource_ThrowsArgumentNullException()
        {
            var sourceStream = CreateNullStream();
            var destinationStream = CreateFakeWriteOnlyStream();

            void copyAction()
            {
                _ = StreamCopyDataExtensions.CopyData(sourceStream, destinationStream, 1);
            }

            _ = Assert.Throws<ArgumentNullException>(copyAction);
        }

        [Fact]
        public async void CopyDataAsync_NullSource_ThrowsArgumentNullException()
        {
            var sourceStream = CreateNullStream();
            var destinationStream = CreateFakeWriteOnlyStream();

            async Task copyAction()
            {
                _ = await StreamCopyDataExtensions.CopyDataAsync(sourceStream, destinationStream, 1, cancellationToken: CancellationToken.None);
            }

            _ = await Assert.ThrowsAsync<ArgumentNullException>(copyAction);
        }

        [Fact]
        public void CopyData_NullDestination_ThrowsArgumentNullException()
        {
            var sourceStream = CreateFakeReadOnlyStream();
            var destinationStream = CreateNullStream();

            void copyAction()
            {
                _ = StreamCopyDataExtensions.CopyData(sourceStream, destinationStream, 1);
            }

            _ = Assert.Throws<ArgumentNullException>(copyAction);
        }

        [Fact]
        public async void CopyDataAsync_NullDestination_ThrowsArgumentNullException()
        {
            var sourceStream = CreateFakeReadOnlyStream();
            var destinationStream = CreateNullStream();

            async Task copyAction()
            {
                _ = await StreamCopyDataExtensions.CopyDataAsync(sourceStream, destinationStream, 1, cancellationToken: CancellationToken.None);
            }

            _ = await Assert.ThrowsAsync<ArgumentNullException>(copyAction);
        }

        [Fact]
        public void CopyData_NonReadableSource_ThrowsNotSupportedException()
        {
            var sourceStream = CreateFakeWriteOnlyStream();
            var destinationStream = CreateFakeWriteOnlyStream();

            void copyAction()
            {
                _ = StreamCopyDataExtensions.CopyData(sourceStream, destinationStream, 1);
            }

            _ = Assert.Throws<NotSupportedException>(copyAction);
        }

        [Fact]
        public async void CopyDataAsync_NonReadableSource_ThrowsNotSupportedException()
        {
            var sourceStream = CreateFakeWriteOnlyStream();
            var destinationStream = CreateFakeWriteOnlyStream();

            async Task copyAction()
            {
                _ = await StreamCopyDataExtensions.CopyDataAsync(sourceStream, destinationStream, 1, cancellationToken: CancellationToken.None);
            }

            _ = await Assert.ThrowsAsync<NotSupportedException>(copyAction);
        }

        [Fact]
        public void CopyData_NonWritableDestination_ThrowsNotSupportedException()
        {
            var sourceStream = CreateFakeReadOnlyStream();
            var destinationStream = CreateFakeReadOnlyStream();

            void copyAction()
            {
                _ = StreamCopyDataExtensions.CopyData(sourceStream, destinationStream, 1);
            }

            _ = Assert.Throws<NotSupportedException>(copyAction);
        }

        [Fact]
        public async void CopyDataAsync_NonWritableDestination_ThrowsNotSupportedException()
        {
            var sourceStream = CreateFakeReadOnlyStream();
            var destinationStream = CreateFakeReadOnlyStream();

            async Task copyAction()
            {
                _ = await StreamCopyDataExtensions.CopyDataAsync(sourceStream, destinationStream, 1, cancellationToken: CancellationToken.None);
            }

            _ = await Assert.ThrowsAsync<NotSupportedException>(copyAction);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(int.MinValue)]
        public void CopyData_ZeroNegativeBufferSize_ThrowsArgumentOutOfRangeException(int bufferSize)
        {
            var sourceStream = CreateFakeReadOnlyStream();
            var destinationStream = CreateFakeWriteOnlyStream();

            void copyAction()
            {
                _ = StreamCopyDataExtensions.CopyData(sourceStream, destinationStream, bufferSize);
            }

            _ = Assert.Throws<ArgumentOutOfRangeException>(copyAction);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(int.MinValue)]
        public async void CopyDataAsync_ZeroNegativeBufferSize_ThrowsArgumentOutOfRangeException(int bufferSize)
        {
            var sourceStream = CreateFakeReadOnlyStream();
            var destinationStream = CreateFakeWriteOnlyStream();

            async Task copyAction()
            {
                _ = await StreamCopyDataExtensions.CopyDataAsync(sourceStream, destinationStream, bufferSize);
            }

            _ = await Assert.ThrowsAsync<ArgumentOutOfRangeException>(copyAction);
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
        public void CopyData_CorrectArguments_CopiesData(byte[] sourceArray, long copyLimit,
            long expectedCopyCount, byte[] expectedDestinationArray)
        {
            using var sourceStream = new MemoryStream(sourceArray, false);
            using var destinationStream = new MemoryStream();

            var copyCount = StreamCopyDataExtensions.CopyData(sourceStream, destinationStream, copyLimit);
            var destinationArray = destinationStream.ToArray();

            Assert.Equal(expectedCopyCount, copyCount);
            Assert.Equal(expectedDestinationArray, destinationArray);
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
            await using var sourceStream = new MemoryStream(sourceArray, false);
            await using var destinationStream = new MemoryStream();

            var copyCount = await StreamCopyDataExtensions.CopyDataAsync(sourceStream, destinationStream, copyLimit);
            var destinationArray = destinationStream.ToArray();

            Assert.Equal(expectedCopyCount, copyCount);
            Assert.Equal(expectedDestinationArray, destinationArray);
        }

        private static Stream CreateNullStream()
        {
            return null;
        }

        private static Stream CreateFakeReadOnlyStream()
        {
            return new FakeStream(canWrite: false);
        }

        private static Stream CreateFakeWriteOnlyStream()
        {
            return new FakeStream(canRead: false);
        }
    }
}