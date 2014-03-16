using Hadouken.SemVer;
using Xunit;

namespace Hadouken.Tests.SemVer
{
    public class SemanticVersionRangeTests
    {
        public class TheTryParseMethod
        {
            [Fact]
            public void Should_Return_False_If_Range_Is_Invalid()
            {
                // Given
                SemanticVersionRange range;
                
                // When
                var result = SemanticVersionRange.TryParse("invalid", out range);

                // Then
                Assert.False(result);
                Assert.Null(range);
            }

            [Fact]
            public void Should_Return_False_If_Range_Lower_Is_Invalid()
            {
                // Given
                SemanticVersionRange range;

                // When
                var result = SemanticVersionRange.TryParse("(invalid,1.0)", out range);

                // Then
                Assert.False(result);
                Assert.Null(range);
            }

            [Fact]
            public void Should_Return_False_If_Range_Upper_Is_Invalid()
            {
                // Given
                SemanticVersionRange range;

                // When
                var result = SemanticVersionRange.TryParse("(1.0,invalid)", out range);

                // Then
                Assert.False(result);
                Assert.Null(range);
            }

            [Fact]
            public void Should_Give_Correct_Rule_When_Range_Is_Single_Version()
            {
                // Given
                SemanticVersionRange range;

                // When
                SemanticVersionRange.TryParse("1.0", out range);

                // Then
                Assert.IsType<GreaterThanOrEqualsRule>(range.Rules[0]);
            }

            [Fact]
            public void Should_Give_Correct_Rule_When_Range_Is_Exclusive_No_Lower_With_Inclusive_Upper()
            {
                // Given
                SemanticVersionRange range;

                // When
                SemanticVersionRange.TryParse("(,1.0]", out range);

                // Then
                Assert.IsType<LessThanOrEqualsRule>(range.Rules[0]);
            }

            [Fact]
            public void Should_Give_Correct_Rule_When_Range_Is_Exclusive_No_Lower_With_Exclusive_Upper()
            {
                // Given
                SemanticVersionRange range;

                // When
                SemanticVersionRange.TryParse("(,1.0)", out range);

                // Then
                Assert.IsType<LessThanRule>(range.Rules[0]);
            }

            [Fact]
            public void Should_Give_Correct_Rule_When_Range_Is_Inclusive_Single_Version()
            {
                // Given
                SemanticVersionRange range;

                // When
                SemanticVersionRange.TryParse("[1.0]", out range);

                // Then
                Assert.IsType<EqualsRule>(range.Rules[0]);
            }

            [Fact]
            public void Should_Give_No_Rule_When_Range_Is_Exclusive_Single_Version()
            {
                // Given
                SemanticVersionRange range;

                // When
                var result = SemanticVersionRange.TryParse("(1.0)", out range);

                // Then
                Assert.False(result);
                Assert.Null(range);
            }

            [Fact]
            public void Should_Give_Correct_Rule_When_Range_Is_Exclusive_Lower_With_Exclusive_No_Upper()
            {
                // Given
                SemanticVersionRange range;

                // When
                SemanticVersionRange.TryParse("(1.0,)", out range);

                // Then
                Assert.IsType<GreaterThanRule>(range.Rules[0]);
            }

            [Fact]
            public void Should_Give_Correct_Rule_When_Range_Is_Exclusive_Lower_With_Exclusive_Upper()
            {
                // Given
                SemanticVersionRange range;

                // When
                SemanticVersionRange.TryParse("(1.0,2.0)", out range);

                // Then
                Assert.IsType<GreaterThanRule>(range.Rules[0]);
                Assert.IsType<LessThanRule>(range.Rules[1]);
            }

            [Fact]
            public void Should_Give_Correct_Rule_When_Range_Is_Inclusive_Lower_With_Inclusive_Upper()
            {
                // Given
                SemanticVersionRange range;

                // When
                SemanticVersionRange.TryParse("[1.0,2.0]", out range);

                // Then
                Assert.IsType<GreaterThanOrEqualsRule>(range.Rules[0]);
                Assert.IsType<LessThanOrEqualsRule>(range.Rules[1]);
            }

            [Fact]
            public void Should_Give_Correct_Rule_When_Range_Is_Inclusive_Lower_With_Exclusive_Upper()
            {
                // Given
                SemanticVersionRange range;

                // When
                SemanticVersionRange.TryParse("[1.0,2.0)", out range);

                // Then
                Assert.IsType<GreaterThanOrEqualsRule>(range.Rules[0]);
                Assert.IsType<LessThanRule>(range.Rules[1]);
            }

            [Fact]
            public void Should_Give_Correct_Rule_When_Range_Is_Exclusive_Lower_With_Inclusive_Upper()
            {
                // Given
                SemanticVersionRange range;

                // When
                SemanticVersionRange.TryParse("(1.0,2.0]", out range);

                // Then
                Assert.IsType<GreaterThanRule>(range.Rules[0]);
                Assert.IsType<LessThanOrEqualsRule>(range.Rules[1]);
            }
        }

        public class TheIsIncludedMethod
        {
            [Fact]
            public void Should_Return_True_For_Same_Version_When_Range_Is_Single_Version()
            {
                // Given
                SemanticVersionRange range;

                // When
                SemanticVersionRange.TryParse("1.0", out range);

                // Then
                Assert.True(range.IsIncluded("1.0"));
            }

            [Fact]
            public void Should_Return_True_For_Higher_Version_When_Range_Is_Single_Version()
            {
                // Given
                SemanticVersionRange range;

                // When
                SemanticVersionRange.TryParse("1.0", out range);

                // Then
                Assert.True(range.IsIncluded("1.1"));
            }

            [Fact]
            public void Should_Return_False_For_Lower_Version_When_Range_Is_Single_Version()
            {
                // Given
                SemanticVersionRange range;

                // When
                SemanticVersionRange.TryParse("1.0", out range);

                // Then
                Assert.False(range.IsIncluded("0.9"));
            }

            [Fact]
            public void Should_Return_True_For_Same_Version_When_Range_Is_Exclusive_No_Lower_With_Inclusive_Upper()
            {
                // Given
                SemanticVersionRange range;

                // When
                SemanticVersionRange.TryParse("(,1.0]", out range);

                // Then
                Assert.True(range.IsIncluded("1.0"));
            }

            [Fact]
            public void Should_Return_True_For_Lower_Version_When_Range_Is_Exclusive_No_Lower_With_Inclusive_Upper()
            {
                // Given
                SemanticVersionRange range;

                // When
                SemanticVersionRange.TryParse("(,1.0]", out range);

                // Then
                Assert.True(range.IsIncluded("0.9"));
            }

            [Fact]
            public void Should_Return_False_For_Higher_Version_When_Range_Is_Exclusive_No_Lower_With_Inclusive_Upper()
            {
                // Given
                SemanticVersionRange range;

                // When
                SemanticVersionRange.TryParse("(,1.0]", out range);

                // Then
                Assert.False(range.IsIncluded("1.1"));
            }

            [Fact]
            public void Should_Return_True_For_Lower_Version_When_Range_Is_Exclusive_No_Lower_With_Exclusive_Upper()
            {
                // Given
                SemanticVersionRange range;

                // When
                SemanticVersionRange.TryParse("(,1.0)", out range);

                // Then
                Assert.True(range.IsIncluded("0.9"));
            }

            [Fact]
            public void Should_Return_False_For_Same_Version_When_Range_Is_Exclusive_No_Lower_With_Exclusive_Upper()
            {
                // Given
                SemanticVersionRange range;

                // When
                SemanticVersionRange.TryParse("(,1.0)", out range);

                // Then
                Assert.False(range.IsIncluded("1.0"));
            }

            [Fact]
            public void Should_Return_False_For_Higher_Version_When_Range_Is_Exclusive_No_Lower_With_Exclusive_Upper()
            {
                // Given
                SemanticVersionRange range;

                // When
                SemanticVersionRange.TryParse("(,1.0)", out range);

                // Then
                Assert.False(range.IsIncluded("1.0"));
            }

            [Fact]
            public void Should_Return_True_For_Same_Version_When_Range_Is_Inclusive_Single_Version()
            {
                // Given
                SemanticVersionRange range;

                // When
                SemanticVersionRange.TryParse("[1.0]", out range);

                // Then
                Assert.True(range.IsIncluded("1.0"));
            }

            [Fact]
            public void Should_Return_False_For_Lower_Version_When_Range_Is_Inclusive_Single_Version()
            {
                // Given
                SemanticVersionRange range;

                // When
                SemanticVersionRange.TryParse("[1.0]", out range);

                // Then
                Assert.False(range.IsIncluded("0.9"));
            }

            [Fact]
            public void Should_Return_False_For_Higher_Version_When_Range_Is_Inclusive_Single_Version()
            {
                // Given
                SemanticVersionRange range;

                // When
                SemanticVersionRange.TryParse("[1.0]", out range);

                // Then
                Assert.False(range.IsIncluded("1.1"));
            }

            [Fact]
            public void Should_Return_True_For_Higher_Version_When_Range_Is_Exclusive_Lower_Exclusive_No_Upper()
            {
                // Given
                SemanticVersionRange range;

                // When
                SemanticVersionRange.TryParse("(1.0,)", out range);

                // Then
                Assert.True(range.IsIncluded("1.1"));
            }

            [Fact]
            public void Should_Return_False_For_Same_Version_When_Range_Is_Exclusive_Lower_Exclusive_No_Upper()
            {
                // Given
                SemanticVersionRange range;

                // When
                SemanticVersionRange.TryParse("(1.0,)", out range);

                // Then
                Assert.False(range.IsIncluded("1.0"));
            }

            [Fact]
            public void Should_Return_False_For_Lower_Version_When_Range_Is_Exclusive_Lower_Exclusive_No_Upper()
            {
                // Given
                SemanticVersionRange range;

                // When
                SemanticVersionRange.TryParse("(1.0,)", out range);

                // Then
                Assert.False(range.IsIncluded("0.9"));
            }

            [Fact]
            public void Should_Return_True_For_Version_Between_Lower_And_Upper_When_Range_Is_Exclusive_Lower_Exclusive_Upper()
            {
                // Given
                SemanticVersionRange range;

                // When
                SemanticVersionRange.TryParse("(1.0,2.0)", out range);

                // Then
                Assert.True(range.IsIncluded("1.5"));
            }

            [Fact]
            public void Should_Return_False_For_Version_Same_As_Lower_When_Range_Is_Exclusive_Lower_Exclusive_Upper()
            {
                // Given
                SemanticVersionRange range;

                // When
                SemanticVersionRange.TryParse("(1.0,2.0)", out range);

                // Then
                Assert.False(range.IsIncluded("1.0"));
            }

            [Fact]
            public void Should_Return_False_For_Version_Lower_Than_Lower_When_Range_Is_Exclusive_Lower_Exclusive_Upper()
            {
                // Given
                SemanticVersionRange range;

                // When
                SemanticVersionRange.TryParse("(1.0,2.0)", out range);

                // Then
                Assert.False(range.IsIncluded("0.9"));
            }

            [Fact]
            public void Should_Return_False_For_Version_Same_As_Upper_When_Range_Is_Exclusive_Lower_Exclusive_Upper()
            {
                // Given
                SemanticVersionRange range;

                // When
                SemanticVersionRange.TryParse("(1.0,2.0)", out range);

                // Then
                Assert.False(range.IsIncluded("2.0"));
            }

            [Fact]
            public void Should_Return_False_For_Version_Higher_Than_Upper_When_Range_Is_Exclusive_Lower_Exclusive_Upper()
            {
                // Given
                SemanticVersionRange range;

                // When
                SemanticVersionRange.TryParse("(1.0,2.0)", out range);

                // Then
                Assert.False(range.IsIncluded("2.1"));
            }

            [Fact]
            public void Should_Return_True_For_Version_Between_Lower_And_Upper_When_Range_Is_Inclusive_Lower_Inclusive_Upper()
            {
                // Given
                SemanticVersionRange range;

                // When
                SemanticVersionRange.TryParse("[1.0,2.0]", out range);

                // Then
                Assert.True(range.IsIncluded("1.5"));
            }

            [Fact]
            public void Should_Return_True_For_Version_Same_As_Lower_When_Range_Is_Inclusive_Lower_Inclusive_Upper()
            {
                // Given
                SemanticVersionRange range;

                // When
                SemanticVersionRange.TryParse("[1.0,2.0]", out range);

                // Then
                Assert.True(range.IsIncluded("1.0"));
            }

            [Fact]
            public void Should_Return_True_For_Version_Same_As_Upper_When_Range_Is_Inclusive_Lower_Inclusive_Upper()
            {
                // Given
                SemanticVersionRange range;

                // When
                SemanticVersionRange.TryParse("[1.0,2.0]", out range);

                // Then
                Assert.True(range.IsIncluded("2.0"));
            }

            [Fact]
            public void Should_Return_False_For_Version_Lower_Than_Lower_When_Range_Is_Inclusive_Lower_Inclusive_Upper()
            {
                // Given
                SemanticVersionRange range;

                // When
                SemanticVersionRange.TryParse("[1.0,2.0]", out range);

                // Then
                Assert.False(range.IsIncluded("0.9"));
            }

            [Fact]
            public void Should_Return_False_For_Version_Higher_Than_Upper_When_Range_Is_Inclusive_Lower_Inclusive_Upper()
            {
                // Given
                SemanticVersionRange range;

                // When
                SemanticVersionRange.TryParse("[1.0,2.0]", out range);

                // Then
                Assert.False(range.IsIncluded("2.1"));
            }

            [Fact]
            public void Should_Return_True_For_Version_Between_Lower_And_Upper_When_Range_Is_Inclusive_Lower_Exclusive_Upper()
            {
                // Given
                SemanticVersionRange range;

                // When
                SemanticVersionRange.TryParse("[1.0,2.0)", out range);

                // Then
                Assert.True(range.IsIncluded("1.5"));
            }

            [Fact]
            public void Should_Return_True_For_Version_Same_As_Lower_When_Range_Is_Inclusive_Lower_Exclusive_Upper()
            {
                // Given
                SemanticVersionRange range;

                // When
                SemanticVersionRange.TryParse("[1.0,2.0)", out range);

                // Then
                Assert.True(range.IsIncluded("1.0"));
            }

            [Fact]
            public void Should_Return_False_For_Version_Lower_Than_Lower_When_Range_Is_Inclusive_Lower_Exclusive_Upper()
            {
                // Given
                SemanticVersionRange range;

                // When
                SemanticVersionRange.TryParse("[1.0,2.0)", out range);

                // Then
                Assert.False(range.IsIncluded("0.9"));
            }

            [Fact]
            public void Should_Return_False_For_Version_Same_As_Upper_When_Range_Is_Inclusive_Lower_Exclusive_Upper()
            {
                // Given
                SemanticVersionRange range;

                // When
                SemanticVersionRange.TryParse("[1.0,2.0)", out range);

                // Then
                Assert.False(range.IsIncluded("2.0"));
            }

            [Fact]
            public void Should_Return_False_For_Version_Higher_Than_Upper_When_Range_Is_Inclusive_Lower_Exclusive_Upper()
            {
                // Given
                SemanticVersionRange range;

                // When
                SemanticVersionRange.TryParse("[1.0,2.0)", out range);

                // Then
                Assert.False(range.IsIncluded("2.1"));
            }

            [Fact]
            public void Should_Return_True_For_Version_Between_Lower_And_Upper_When_Range_Is_Exclusive_Lower_Inclusive_Upper()
            {
                // Given
                SemanticVersionRange range;

                // When
                SemanticVersionRange.TryParse("(1.0,2.0]", out range);

                // Then
                Assert.True(range.IsIncluded("1.5"));
            }

            [Fact]
            public void Should_Return_True_For_Version_Same_As_Upper_When_Range_Is_Exclusive_Lower_Inclusive_Upper()
            {
                // Given
                SemanticVersionRange range;

                // When
                SemanticVersionRange.TryParse("(1.0,2.0]", out range);

                // Then
                Assert.True(range.IsIncluded("2.0"));
            }

            [Fact]
            public void Should_Return_False_For_Version_Higher_Than_Upper_When_Range_Is_Exclusive_Lower_Inclusive_Upper()
            {
                // Given
                SemanticVersionRange range;

                // When
                SemanticVersionRange.TryParse("(1.0,2.0]", out range);

                // Then
                Assert.False(range.IsIncluded("2.1"));
            }

            [Fact]
            public void Should_Return_False_For_Version_Same_As_Lower_When_Range_Is_Exclusive_Lower_Inclusive_Upper()
            {
                // Given
                SemanticVersionRange range;

                // When
                SemanticVersionRange.TryParse("(1.0,2.0]", out range);

                // Then
                Assert.False(range.IsIncluded("1.0"));
            }

            [Fact]
            public void Should_Return_False_For_Version_Lower_Than_Lower_When_Range_Is_Exclusive_Lower_Inclusive_Upper()
            {
                // Given
                SemanticVersionRange range;

                // When
                SemanticVersionRange.TryParse("(1.0,2.0]", out range);

                // Then
                Assert.False(range.IsIncluded("0.9"));
            }
        }
    }
}
