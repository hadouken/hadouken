using Hadouken.Framework.SemVer;
using NUnit.Framework;

namespace Hadouken.Framework.Tests.SemVer
{
    public class SemanticVersionRangeTests
    {
        [Test]
        public void TryParse_SingleVersion()
        {
            SemanticVersionRange range;
            SemanticVersionRange.TryParse("1.0", out range);

            Assert.AreEqual(1, range.Rules.Length);
            Assert.AreEqual(typeof (GreaterThanOrEqualsRule), range.Rules[0].GetType());
        }

        [Test]
        public void TryParse_ExclusiveNoLowerInclusiveUpper()
        {
            SemanticVersionRange range;
            SemanticVersionRange.TryParse("(,1.0]", out range);

            Assert.AreEqual(1, range.Rules.Length);
            Assert.AreEqual(typeof (LessThanOrEqualsRule), range.Rules[0].GetType());
        }

        [Test]
        public void TryParse_ExlusiveNoLowerExclusiveUpper()
        {
            SemanticVersionRange range;
            SemanticVersionRange.TryParse("(,1.0)", out range);

            Assert.AreEqual(1, range.Rules.Length);
            Assert.AreEqual(typeof (LessThanRule), range.Rules[0].GetType());
        }

        [Test]
        public void TryParse_InclusiveSingleVersion()
        {
            SemanticVersionRange range;
            SemanticVersionRange.TryParse("[1.0]", out range);

            Assert.AreEqual(1, range.Rules.Length);
            Assert.AreEqual(typeof (EqualsRule), range.Rules[0].GetType());
        }

        [Test]
        public void TryParse_ExclusiveSingleVersion()
        {
            SemanticVersionRange range;
            SemanticVersionRange.TryParse("(1.0)", out range);

            Assert.IsNull(range);
        }

        [Test]
        public void TryParse_ExclusiveLowerExclusiveNoUpper()
        {
            SemanticVersionRange range;
            SemanticVersionRange.TryParse("(1.0,)", out range);

            Assert.AreEqual(1, range.Rules.Length);
            Assert.AreEqual(typeof (GreaterThanRule), range.Rules[0].GetType());
        }

        [Test]
        public void TryParse_ExclusiveLowerExclusiveUpper()
        {
            SemanticVersionRange range;
            SemanticVersionRange.TryParse("(1.0,2.0)", out range);

            Assert.AreEqual(2, range.Rules.Length);
            Assert.AreEqual(typeof (GreaterThanRule), range.Rules[0].GetType());
            Assert.AreEqual(typeof (LessThanRule), range.Rules[1].GetType());
        }

        [Test]
        public void TryParse_InclusiveLowerInclusiveUpper()
        {
            SemanticVersionRange range;
            SemanticVersionRange.TryParse("[1.0,2.0]", out range);

            Assert.AreEqual(2, range.Rules.Length);
            Assert.AreEqual(typeof (GreaterThanOrEqualsRule), range.Rules[0].GetType());
            Assert.AreEqual(typeof (LessThanOrEqualsRule), range.Rules[1].GetType());
        }

        [Test]
        public void TryParse_InclusiveLowerExclusiveUpper()
        {
            SemanticVersionRange range;
            SemanticVersionRange.TryParse("[1.0,2.0)", out range);

            Assert.AreEqual(2, range.Rules.Length);
            Assert.AreEqual(typeof(GreaterThanOrEqualsRule), range.Rules[0].GetType());
            Assert.AreEqual(typeof(LessThanRule), range.Rules[1].GetType());
        }

        [Test]
        public void TryParse_ExclusiveLowerInclusiveUpper()
        {
            SemanticVersionRange range;
            SemanticVersionRange.TryParse("(1.0,2.0]", out range);

            Assert.AreEqual(2, range.Rules.Length);
            Assert.AreEqual(typeof(GreaterThanRule), range.Rules[0].GetType());
            Assert.AreEqual(typeof(LessThanOrEqualsRule), range.Rules[1].GetType());
        }

        [Test]
        public void TryParse_SilentlyHandlesGarbageRange()
        {
            SemanticVersionRange range;
            Assert.DoesNotThrow(() => SemanticVersionRange.TryParse("/garbage", out range));
        }

        [Test]
        public void TryParse_SilentlyHandlesInvalidLower()
        {
            SemanticVersionRange range;
            Assert.DoesNotThrow(() => SemanticVersionRange.TryParse("(garbage,1.0)", out range));
        }

        [Test]
        public void TryParse_SilentlyHandlesInvalidUpper()
        {
            SemanticVersionRange range;
            Assert.DoesNotThrow(() => SemanticVersionRange.TryParse("(1.0,garbage)", out range));
        }

        [Test]
        public void IsIncluded_SingleVersion()
        {
            SemanticVersionRange range;
            SemanticVersionRange.TryParse("1.0", out range);

            Assert.IsTrue(range.IsIncluded("1.0"));
            Assert.IsTrue(range.IsIncluded("1.1"));
            Assert.IsFalse(range.IsIncluded("0.9"));
        }

        [Test]
        public void IsIncluded_ExclusiveNoLowerInclusiveUpper()
        {
            SemanticVersionRange range;
            SemanticVersionRange.TryParse("(,1.0]", out range);

            Assert.IsTrue(range.IsIncluded("1.0"));
            Assert.IsTrue(range.IsIncluded("0.9"));
            Assert.IsFalse(range.IsIncluded("1.1"));
        }

        [Test]
        public void IsIncluded_ExclusiveNoLowerExclusiveUpper()
        {
            SemanticVersionRange range;
            SemanticVersionRange.TryParse("(,1.0)", out range);

            Assert.IsTrue(range.IsIncluded("0.9"));
            Assert.IsFalse(range.IsIncluded("1.0"));
            Assert.IsFalse(range.IsIncluded("1.1"));
        }

        [Test]
        public void IsIncluded_InclusiveSingleVersion()
        {
            SemanticVersionRange range;
            SemanticVersionRange.TryParse("[1.0]", out range);

            Assert.IsTrue(range.IsIncluded("1.0"));
            Assert.IsFalse(range.IsIncluded("1.1"));
            Assert.IsFalse(range.IsIncluded("0.9"));
        }

        [Test]
        public void IsIncluded_ExclusiveLowerExclusiveNoUpper()
        {
            SemanticVersionRange range;
            SemanticVersionRange.TryParse("(1.0,)", out range);

            Assert.IsTrue(range.IsIncluded("1.1"));
            Assert.IsFalse(range.IsIncluded("1.0"));
            Assert.IsFalse(range.IsIncluded("0.9"));
        }

        [Test]
        public void IsIncluded_ExclusiveLowerExclusiveUpper()
        {
            SemanticVersionRange range;
            SemanticVersionRange.TryParse("(1.0,2.0)", out range);

            Assert.IsTrue(range.IsIncluded("1.5"));
            Assert.IsFalse(range.IsIncluded("1.0"));
            Assert.IsFalse(range.IsIncluded("2.0"));
        }

        [Test]
        public void IsIncluded_InclusiveLowerInclusiveUpper()
        {
            SemanticVersionRange range;
            SemanticVersionRange.TryParse("[1.0,2.0]", out range);

            Assert.IsTrue(range.IsIncluded("1.0"));
            Assert.IsTrue(range.IsIncluded("1.5"));
            Assert.IsTrue(range.IsIncluded("2.0"));
            Assert.IsFalse(range.IsIncluded("0.9"));
            Assert.IsFalse(range.IsIncluded("2.1"));
        }

        [Test]
        public void IsIncluded_InclusiveLowerExclusiveUpper()
        {
            SemanticVersionRange range;
            SemanticVersionRange.TryParse("[1.0,2.0)", out range);

            Assert.IsTrue(range.IsIncluded("1.0"));
            Assert.IsTrue(range.IsIncluded("1.5"));
            Assert.IsFalse(range.IsIncluded("2.0"));
            Assert.IsFalse(range.IsIncluded("2.1"));
        }

        [Test]
        public void IsIncluded_ExclusiveLowerInclusiveUpper()
        {
            SemanticVersionRange range;
            SemanticVersionRange.TryParse("(1.0,2.0]", out range);

            Assert.IsTrue(range.IsIncluded("1.5"));
            Assert.IsTrue(range.IsIncluded("2.0"));
            Assert.IsFalse(range.IsIncluded("1.0"));
            Assert.IsFalse(range.IsIncluded("2.1"));
        }
    }
}
