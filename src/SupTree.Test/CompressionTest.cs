using NUnit.Framework;
using SupTree.Common;

namespace SupTree.Test
{
    [TestFixture]
    internal class CompressionTest
    {
        [Test]
        public void CanCompressAndUncompressUsingGZip()
        {
            var largeString = LargeString.GetLargeString;

            var bytes = Compression.GZip(largeString);
            var newLargeString = Compression.UnGZip(bytes);

            Assert.That(newLargeString, Is.EqualTo(largeString));
        }
    }
}
