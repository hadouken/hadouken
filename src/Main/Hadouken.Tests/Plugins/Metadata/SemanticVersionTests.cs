using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hadouken.Plugins.Metadata;
using NUnit.Framework;

namespace Hadouken.Tests.Plugins.Metadata
{
    public class SemanticVersionTests
    {
        [Test]
        public void CanConstructSemanticVersionFromDefault()
        {
            var semver = new SemanticVersion();
            Assert.AreEqual("0.0.0", semver.ToString());
        }

        [Test]
        public void CanConstructSemanticVersionFromMajor()
        {
            var semver = new SemanticVersion(1);
            Assert.AreEqual("1.0.0", semver.ToString());
        }

        [Test]
        public void CanConstructSemanticVersionFromMajorMinor()
        {
            var semver = new SemanticVersion(1, 1);
            Assert.AreEqual("1.1.0", semver.ToString());
        }

        [Test]
        public void CanConstructSemanticVersionFromMajorMinorPatch()
        {
            var semver = new SemanticVersion(1, 1, 1);
            Assert.AreEqual("1.1.1", semver.ToString());
        }

        [Test]
        public void CanConstructSemanticVersionFromMajorMinorPatchLabel()
        {
            var semver = new SemanticVersion(1, 1, 1, "label");
            Assert.AreEqual("1.1.1-label", semver.ToString());
        }

        [Test]
        public void CanConstructSemanticVersionFromString()
        {
            var semver = new SemanticVersion("1.0.1-label");
            Assert.AreEqual("1.0.1-label", semver.ToString());
        }

        [Test]
        public void LessThanComparisonWithMajorMinorSucceeds()
        {
            var lower = new SemanticVersion("1.0");
            var upper = new SemanticVersion("1.1");

            Assert.IsTrue(lower < upper);
        }

        [Test]
        public void LessThanComparisonWithMajorMinorPatchSucceeds()
        {
            var lower = new SemanticVersion("1.1.132");
            var upper = new SemanticVersion("1.1.133");

            Assert.IsTrue(lower < upper);
        }

        [Test]
        public void LessThanComparisonWithMajorMinorPatchLabelSucceeds()
        {
            var lower = new SemanticVersion("0.1.123-label.1");
            var upper = new SemanticVersion("1.1.2-label.2");

            Assert.IsTrue(lower < upper);
        }

        [Test]
        public void LessThanComparisonWithOneSideLabelSucceeds()
        {
            var lower = new SemanticVersion("1.1.1-abc");
            var upper = new SemanticVersion("1.1.1");

            Assert.IsTrue(lower < upper);
        }

        [Test]
        public void ImplicitStringOperatorSucceeds()
        {
            SemanticVersion semver = "1.2.3-label";
            Assert.AreEqual("1.2.3-label", semver.ToString());
        }

        [Test]
        public void SetsCorrectLabel()
        {
            var semver = new SemanticVersion("1.9.0-sweet.label");
            Assert.AreEqual("sweet.label", semver.Label);
        }

        [Test]
        public void DifferentInstancesWithSameVersionAreEqual()
        {
            var semver1 = new SemanticVersion("1.0.0-foo");
            var semver2 = new SemanticVersion("1.0.0-foo");

            Assert.AreEqual(semver1, semver2);
            Assert.IsTrue(semver1 == semver2);
            Assert.IsTrue(semver1.Equals(semver2));
        }
    }
}
