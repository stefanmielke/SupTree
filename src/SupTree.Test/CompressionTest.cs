using System.Text;
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

            var bytes = Compression.GZip(largeString, Encoding.UTF8);
            var newLargeString = Compression.UnGZip(bytes, Encoding.UTF8);

            Assert.That(newLargeString, Is.EqualTo(largeString));
        }
    }
}
