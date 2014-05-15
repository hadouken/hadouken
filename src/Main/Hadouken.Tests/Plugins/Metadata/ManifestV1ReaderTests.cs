using System.Linq;
using Hadouken.Plugins.Metadata;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Hadouken.Tests.Plugins.Metadata
{
    public class ManifestV1ReaderTests
    {
        public class TheReadMethod
        {
            [Fact]
            public void Should_Return_Manifest_For_Valid_Json()
            {
                // Given
                var json = JObject.Parse("{ \"id\": \"test\", \"version\": \"1.0.0\" }");
                var reader = new ManifestV1Reader();

                // When
                var manifest = reader.Read(json);

                // Then
                Assert.NotNull(manifest);
                Assert.Equal("test", manifest.Name);
                Assert.Equal("1.0.0", manifest.Version.ToString());
            }

            [Fact]
            public void Should_Return_Manifest_For_Valid_Json_With_Dependencies()
            {
                // Given
                var json = JObject.Parse("{ \"id\": \"test\", \"version\": \"1.0.0\", \"dependencies\": [ { \"id\": \"test-dependency\", \"version\": \"1.0\" } ] }");
                var reader = new ManifestV1Reader();

                // When
                var manifest = reader.Read(json);

                // Then
                Assert.NotNull(manifest);
                Assert.Equal("test", manifest.Name);
                Assert.Equal("1.0.0", manifest.Version.ToString());
                Assert.Equal(1, manifest.Dependencies.Count());
            }

            [Fact]
            public void Should_Throw_Exception_When_Missing_Id()
            {
                // Given
                var json = JObject.Parse("{ \"version\": \"1.0.0\" }");
                var reader = new ManifestV1Reader();

                // When
                var exception = Assert.Throws<MissingPropertyException>(() => reader.Read(json));

                // Then
                Assert.Equal("id", exception.FieldName);
            }

            [Fact]
            public void Should_Throw_Exception_When_Missing_Version()
            {
                // Given
                var json = JObject.Parse("{ \"id\": \"test\" }");
                var reader = new ManifestV1Reader();

                // When
                var exception = Assert.Throws<MissingPropertyException>(() => reader.Read(json));

                // Then
                Assert.Equal("version", exception.FieldName);
            }
        }
    }
}
