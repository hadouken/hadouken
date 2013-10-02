using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hadouken.Plugins.Metadata;
using NUnit.Framework;

namespace Hadouken.Tests.Plugins.Metadata
{
    public class SemanticVersionRangeTests
    {
        [Test]
        public void SingleVersion()
        {
            var range = SemanticVersionRange.Construct("1.0");

            Assert.IsFalse(range.IsIncluded("1.1"));
            Assert.IsTrue(range.IsIncluded("1.0"));
            Assert.IsTrue(range.IsIncluded("1.0-beta"));
            Assert.IsTrue(range.IsIncluded("0.9"));
            Assert.IsTrue(range.IsIncluded("0.9-beta"));
        }
    }
}
