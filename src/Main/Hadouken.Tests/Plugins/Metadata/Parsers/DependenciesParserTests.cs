using System.IO;
using System.Linq;
using Hadouken.Plugins.Metadata;
using Hadouken.Plugins.Metadata.Parsers;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Hadouken.Tests.Plugins.Metadata.Parsers
{
    public class DependenciesParserTests
    {
        public class TheParseMethod
        {
            [Fact]
            public void Should_Return_Empty_List_If_Array_Is_Null()
            {
                // Given
                var json = JArray.Parse("[]");
                var parser = new DependenciesParser();

                // When
                var result = parser.Parse(json);

                // Then
                Assert.Equal(0, result.Count());
            }

            [Fact]
            public void Should_Throw_InvalidDataException_If_Value_Is_Wrong_Type()
            {
                // Given
                var json = JToken.Parse("{ \"foo\": 2 }");
                var parser = new DependenciesParser();

                // When, Then
                Assert.Throws<InvalidDataException>(() => parser.Parse(json));
            }

            [Fact]
            public void Should_Throw_InvalidDataException_If_Any_Array_Elements_Are_Not_Objects()
            {
                // Given
                var json = JToken.Parse("[ { \"foo\": 2 }, 2, 3 ]");
                var parser = new DependenciesParser();

                // When, Then
                Assert.Throws<InvalidDataException>(() => parser.Parse(json));
            }

            [Fact]
            public void Should_Throw_UnexpectedPropertyException_If_Array_Element_Contains_Invalid_Property()
            {
                // Given
                var json = JToken.Parse("[ { \"foo\": 2 } ]");
                var parser = new DependenciesParser();

                // When
                var exception = Assert.Throws<UnexpectedPropertyException>(() => parser.Parse(json));

                // Then
                Assert.Equal("foo", exception.PropertyName);
            }
        }
    }
}
