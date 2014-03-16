using System;
using Hadouken.SemVer;
using Xunit;

namespace Hadouken.Tests.SemVer
{
    public class SemanticVersionTests
    {
        public class TheConstructor
        {
            [Fact]
            public void Should_Set_Correct_Version_When_Given_No_Arguments()
            {
                // Given
                var version = new SemanticVersion();

                // When, Then
                Assert.Equal("0.0.0", version.ToString());
            }

            [Fact]
            public void Should_Set_Correct_Version_When_Given_Major()
            {
                // Given
                var version = new SemanticVersion(1);

                // When, Then
                Assert.Equal("1.0.0", version.ToString());
            }

            [Fact]
            public void Should_Set_Correct_Version_When_Given_Major_Minor()
            {
                // Given
                var version = new SemanticVersion(1, 1);

                // When, Then
                Assert.Equal("1.1.0", version.ToString());
            }

            [Fact]
            public void Should_Set_Correct_Version_When_Given_Major_Minor_Patch()
            {
                // Given
                var version = new SemanticVersion(1, 1, 1);

                // When, Then
                Assert.Equal("1.1.1", version.ToString());
            }

            [Fact]
            public void Should_Set_Correct_Version_When_Given_Major_Minor_Patch_Label()
            {
                // Given
                var version = new SemanticVersion(1, 1, 1, "test");

                // When, Then
                Assert.Equal("1.1.1-test", version.ToString());
            }
        }

        public class TheMajorProperty
        {
            [Fact]
            public void Should_Return_Major_Version()
            {
                // Given
                var version = new SemanticVersion(1);

                // When, Then
                Assert.Equal(1, version.Major);
            }

            [Fact]
            public void Should_Accept_Any_Positive_Integer()
            {
                // Given
                var version = new SemanticVersion();

                // When
                version.Major = 1;

                // Then
                Assert.Equal(1, version.Major);
            }

            [Fact]
            public void Should_Throw_ArgumentOutOfRangeException_If_Setting_Negative_Value()
            {
                // Given
                var version = new SemanticVersion();

                // When, Then
                Assert.Throws<ArgumentOutOfRangeException>(() => version.Major = -1);
            }
        }

        public class TheMinorProperty
        {
            [Fact]
            public void Should_Return_Minor_Version()
            {
                // Given
                var version = new SemanticVersion(1, 1);

                // When, Then
                Assert.Equal(1, version.Minor);
            }

            [Fact]
            public void Should_Accept_Any_Positive_Integer()
            {
                // Given
                var version = new SemanticVersion();

                // When
                version.Minor = 1;

                // Then
                Assert.Equal(1, version.Minor);
            }

            [Fact]
            public void Should_Throw_ArgumentOutOfRangeException_If_Setting_Negative_Value()
            {
                // Given
                var version = new SemanticVersion();

                // When, Then
                Assert.Throws<ArgumentOutOfRangeException>(() => version.Minor = -1);
            }
        }

        public class ThePatchProperty
        {
            [Fact]
            public void Should_Return_The_Patch_Version()
            {
                // Given
                var version = new SemanticVersion(0, 0, 1);

                // When, Then
                Assert.Equal(1, version.Patch);
            }

            [Fact]
            public void Should_Accept_Any_Positive_Integer()
            {
                // Given
                var version = new SemanticVersion();

                // When
                version.Patch = 1;

                // Then
                Assert.Equal(1, version.Patch);
            }

            [Fact]
            public void Should_Throw_ArgumentOutOfRangeException_If_Setting_Negative_Value()
            {
                // Given
                var version = new SemanticVersion();

                // When, Then
                Assert.Throws<ArgumentOutOfRangeException>(() => version.Patch = -1);
            }
        }

        public class TheLessThanOperator
        {
            [Fact]
            public void Should_Throw_ArgumentNullException_If_Left_Value_Is_Null()
            {
                // Given
                var left = (SemanticVersion) null;
                var right = new SemanticVersion();

                // When, Then
                Assert.Throws<ArgumentNullException>(() => left < right);
            }

            [Fact]
            public void Should_Throw_ArgumentNullException_If_Right_Value_Is_Null()
            {
                // Given
                var left = new SemanticVersion();
                var right = (SemanticVersion) null;

                // When, Then
                Assert.Throws<ArgumentNullException>(() => left < right);
            }

            [Fact]
            public void Should_Return_True_When_Left_Major_Is_Less_Than_Right_Major()
            {
                // Given
                var left = new SemanticVersion(1);
                var right = new SemanticVersion(2);

                // When, Then
                Assert.True(left < right);
            }

            [Fact]
            public void Should_Return_False_When_Left_Major_Is_Greater_Than_Right_Major()
            {
                // Given
                var left = new SemanticVersion(2);
                var right = new SemanticVersion(1);

                // When, Then
                Assert.False(left < right);
            }

            [Fact]
            public void Should_Return_True_When_Major_Are_Same_And_Left_Minor_Is_Less_Than_Right_Minor()
            {
                // Given
                var left = new SemanticVersion(1, 0);
                var right = new SemanticVersion(1, 1);

                // When, Then
                Assert.True(left < right);
            }

            [Fact]
            public void Should_Return_False_When_Major_Are_Same_And_Left_Minor_Is_Greater_Than_Right_Minor()
            {
                // Given
                var left = new SemanticVersion(1, 1);
                var right = new SemanticVersion(1, 0);

                // When, Then
                Assert.False(left < right);
            }

            [Fact]
            public void Should_Return_True_When_Major_Minor_Are_Same_And_Left_Patch_Is_Less_Than_Right_Patch()
            {
                // Given
                var left = new SemanticVersion(1, 0, 0);
                var right = new SemanticVersion(1, 0, 1);

                // When, Then
                Assert.True(left < right);
            }

            [Fact]
            public void Should_Return_False_When_Major_Minor_Are_Same_And_Left_Patch_Is_Greater_Than_Right_Patch()
            {
                // Given
                var left = new SemanticVersion(1, 0, 1);
                var right = new SemanticVersion(1, 0, 0);

                // When, Then
                Assert.False(left < right);
            }

            [Fact]
            public void Should_Return_True_When_Major_Minor_Patch_Are_Same_And_Left_Has_Label_When_Right_Has_Not()
            {
                // Given
                var left = new SemanticVersion(1, 0, 0, "a");
                var right = new SemanticVersion(1, 0, 0);

                // When, Then
                Assert.True(left < right);
            }

            [Fact]
            public void Should_Return_False_When_Major_Minor_Patch_Are_Same_And_Left_Has_No_Label_When_Right_Has()
            {
                // Given
                var left = new SemanticVersion(1, 0, 0);
                var right = new SemanticVersion(1, 0, 0, "a");

                // When, Then
                Assert.False(left < right);
            }

            [Fact]
            public void Should_Return_True_When_Major_Minor_Patch_Are_Same_And_Left_Label_Is_Less_Than_Right_Label()
            {
                // Given
                var left = new SemanticVersion(1, 0, 0, "a");
                var right = new SemanticVersion(1, 0, 0, "b");

                // When, Then
                Assert.True(left < right);
            }

            [Fact]
            public void Should_Return_False_When_Major_Minor_Patch_Are_Same_And_Left_Label_Is_Greater_Than_Right_Label()
            {
                // Given
                var left = new SemanticVersion(1, 0, 0, "b");
                var right = new SemanticVersion(1, 0, 0, "a");

                // When, Then
                Assert.False(left < right);
            }
        }

        public class TheGreaterThanOperator
        {
            [Fact]
            public void Should_Throw_ArgumentNullException_When_Left_Value_Is_Null()
            {
                // Given
                var left = (SemanticVersion)null;
                var right = new SemanticVersion();

                // When, Then
                Assert.Throws<ArgumentNullException>(() => left > right);
            }

            [Fact]
            public void Should_Throw_ArgumentNullException_When_Right_Value_Is_Null()
            {
                // Given
                var left = new SemanticVersion();
                var right = (SemanticVersion)null;

                // When, Then
                Assert.Throws<ArgumentNullException>(() => left > right);
            }

            [Fact]
            public void Should_Return_True_When_Left_Major_Is_Greater_Than_Right_Major()
            {
                // Given
                var left = new SemanticVersion(2);
                var right = new SemanticVersion(1);

                // When, Then
                Assert.True(left > right);
            }

            [Fact]
            public void Should_Return_False_When_Left_Major_Is_Less_Than_Right_Major()
            {
                // Given
                var left = new SemanticVersion(1);
                var right = new SemanticVersion(2);

                // When, Then
                Assert.False(left > right);
            }

            [Fact]
            public void Should_Return_True_When_Major_Are_Same_And_Left_Minor_Is_Greater_Than_Right_Minor()
            {
                // Given
                var left = new SemanticVersion(1, 1);
                var right = new SemanticVersion(1, 0);

                // When, Then
                Assert.True(left > right);
            }

            [Fact]
            public void Should_Return_False_When_Major_Are_Same_And_Left_Minor_Is_Lower_Than_Right_Minor()
            {
                // Given
                var left = new SemanticVersion(1, 0);
                var right = new SemanticVersion(1, 1);

                // When, Then
                Assert.False(left > right);
            }

            [Fact]
            public void Should_Return_True_When_Major_Minor_Are_Same_And_Left_Patch_Is_Greater_Than_Right_Patch()
            {
                // Given
                var left = new SemanticVersion(1, 0, 1);
                var right = new SemanticVersion(1, 0, 0);

                // When, Then
                Assert.True(left > right);
            }

            [Fact]
            public void Should_Return_False_When_Major_Minor_Are_Same_And_Left_Patch_Is_Lower_Than_Right_Patch()
            {
                // Given
                var left = new SemanticVersion(1, 0, 0);
                var right = new SemanticVersion(1, 0, 1);

                // When, Then
                Assert.False(left > right);
            }

            [Fact]
            public void Should_Return_True_When_Major_Minor_Patch_Are_Same_And_Left_Has_No_Label_When_Right_Has()
            {
                // Given
                var left = new SemanticVersion(1, 0, 0);
                var right = new SemanticVersion(1, 0, 0, "a");

                // When, Then
                Assert.True(left > right);
            }

            [Fact]
            public void Should_Return_False_When_Major_Minor_Patch_Are_Same_And_Left_Has_Label_When_Right_Has_Not()
            {
                // Given
                var left = new SemanticVersion(1, 0, 0, "a");
                var right = new SemanticVersion(1, 0, 0);

                // When, Then
                Assert.False(left > right);
            }

            [Fact]
            public void Should_Return_True_When_Major_Minor_Patch_Are_Same_And_Left_Label_Is_Greater_Than_Right_Label()
            {
                // Given
                var left = new SemanticVersion(1, 0, 0, "b");
                var right = new SemanticVersion(1, 0, 0, "a");

                // When, Then
                Assert.True(left > right);
            }

            [Fact]
            public void Should_Return_False_When_Major_Minor_Patch_Are_Same_And_Left_Label_Is_Less_Than_Right_Label()
            {
                // Given
                var left = new SemanticVersion(1, 0, 0, "a");
                var right = new SemanticVersion(1, 0, 0, "b");

                // When, Then
                Assert.False(left > right);
            }
        }

        public class TheEqualsOperator
        {
            [Fact]
            public void Should_Return_True_When_Left_And_Right_Are_Same_Instance()
            {
                // Given
                var left = new SemanticVersion();
                var right = left;

                // When, Then
                Assert.True(left == right);
            }

            [Fact]
            public void Should_Return_False_If_Left_Is_Null()
            {
                // Given
                var left = (SemanticVersion) null;
                var right = new SemanticVersion();

                // When, Then
                Assert.False(left == right);
            }

            [Fact]
            public void Should_Return_False_If_Right_Is_Null()
            {
                // Given
                var left = new SemanticVersion();
                var right = (SemanticVersion) null;

                // When, Then
                Assert.False(left == right);
            }

            [Fact]
            public void Should_Return_True_When_Left_And_Right_Are_Same_Version()
            {
                // Given
                var left = new SemanticVersion(1, 0, 0, "a");
                var right = new SemanticVersion(1, 0, 0, "a");

                // When, Then
                Assert.True(left == right);
            }

            [Fact]
            public void Should_Return_False_When_Left_And_Right_Differs()
            {
                // Given
                var left = new SemanticVersion(1, 0, 1, "a");
                var right = new SemanticVersion(1, 2, 1, "c");

                // When, Then
                Assert.False(left == right);
            }
        }

        public class TheImplicitStringOperator
        {
            [Fact]
            public void Should_Throw_ArgumentNullException_When_Argument_Is_Null()
            {
                // Given
                var version = (SemanticVersion) null;

                // When, Then
                Assert.Throws<ArgumentNullException>(() => { string v = version; });
            }

            [Fact]
            public void Should_Return_Correctly_Presented_String_When_Given_A_Valid_Version()
            {
                // Given
                var version = new SemanticVersion(1, 2, 3, "label");

                // When
                string v = version;

                // Then
                Assert.Equal("1.2.3-label", v);
            }
        }
    }
}
