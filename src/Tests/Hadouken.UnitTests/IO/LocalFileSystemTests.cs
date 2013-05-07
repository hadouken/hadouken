using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hadouken.Impl.IO;
using NUnit.Framework;

namespace Hadouken.UnitTests.IO
{
    [TestFixture]
    public class LocalFileSystemTests
    {
        [Test]
        [TestCase(@"C:")]
        [TestCase(@"C:\")]
        [TestCase(@"C:\Temporary")]
        public void Returns_positive_value_on_valid_path(string path)
        {
            var fs = new LocalFileSystem();

            var result = fs.RemainingDiskSpace(path);

            Assert.IsTrue(result > 0);
        }

        [Test]
        public void Returns_negative_value_on_invalid_path()
        {
            var fs = new LocalFileSystem();
            Assert.IsTrue(fs.RemainingDiskSpace("Invalid:\\Invalid") == -1);
        }
    }
}
