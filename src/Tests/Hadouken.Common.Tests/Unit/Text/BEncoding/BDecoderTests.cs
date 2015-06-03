using Hadouken.Common.Text.BEncoding;
using Xunit;
using Xunit.Extensions;

namespace Hadouken.Common.Tests.Unit.Text.BEncoding {
    public sealed class BDecoderTests {
        public sealed class TheDecodeMethod {
            [Theory]
            [InlineData("4:spam", "spam")]
            [InlineData("8:spamspam", "spamspam")]
            public void Can_Decode_String(string given, string expected) {
                // Given, When
                var result = (BEncodedString) new BDecoder().Decode(given);

                // Then
                Assert.Equal(expected, (string) result);
            }

            [Theory]
            [InlineData("i4e", 4)]
            [InlineData("i-10e", -10)]
            public void Can_Decode_Number(string given, long expected) {
                // Given, When
                var result = (BEncodedNumber) new BDecoder().Decode(given);

                // Then
                Assert.Equal(expected, (long) result);
            }

            [Theory]
            [InlineData("le", 0)]
            [InlineData("l4:spame", 1)]
            [InlineData("li10e4:spame", 2)]
            [InlineData("llee", 1)]
            public void Can_Decode_List(string given, int expectedCount) {
                // Given, When
                var result = (BEncodedList) new BDecoder().Decode(given);

                // Then
                Assert.Equal(expectedCount, result.Count);
            }

            [Theory]
            [InlineData("de", 0)]
            [InlineData("d4:spamlee", 1)]
            [InlineData("d4:spamle3:hami10ee", 2)]
            public void Can_Decode_Dictionary(string given, int expectedCount) {
                // Given, When
                var result = (BEncodedDictionary) new BDecoder().Decode(given);

                // Then
                Assert.Equal(expectedCount, result.Count);
            }
        }
    }
}