using System;
using System.Linq;
using Hadouken.Common.BitTorrent;
using Hadouken.Extensions.AutoMove.Data.Models;
using Hadouken.Extensions.AutoMove.Tests.Fixtures;
using NSubstitute;
using Xunit;

namespace Hadouken.Extensions.AutoMove.Tests.Unit
{
    public sealed class ParameterValueReplacerTests
    {
        public sealed class TheConstructor
        {
            [Fact]
            public void Should_Throw_Exception_If_Source_Value_Provider_Is_Null()
            {
                // Given, When
                var exception = Record.Exception(() => new ParameterValueReplacer(null));

                // Then
                Assert.IsType<ArgumentNullException>(exception);
                Assert.Equal("sourceValueProvider", ((ArgumentNullException) exception).ParamName);
            }
        }

        public sealed class TheReplaceMethod
        {
            [Fact]
            public void Should_Not_Replace_Parameter_That_Does_Not_Exist()
            {
                // Given
                var replacer = new ParameterValueReplacerFixture().CreateReplacer();
                var torrent = Substitute.For<ITorrent>();

                // When
                var result = replacer.Replace(torrent, Enumerable.Empty<Parameter>(), "//target-path/${Param}");

                // Then
                Assert.Equal("//target-path/${Param}", result);
            }

            [Fact]
            public void Should_Replace_A_Single_Parameter_With_The_Captured_Value()
            {
                // Given
                var replacer = new ParameterValueReplacerFixture().CreateReplacer();
                var torrent = Substitute.For<ITorrent>();
                torrent.Label.Returns("Test label");

                var parameters = new[]
                {
                    new Parameter {Pattern = "^(?<label>.*)$", Source = ParameterSource.Label}
                };

                // When
                var result = replacer.Replace(torrent, parameters, "//target-path/${label}");

                // Then
                Assert.Equal("//target-path/Test label", result);
            }

            [Fact]
            public void Should_Replace_Multiple_Parameters_With_Their_Captured_Values()
            {
                // Given
                var replacer = new ParameterValueReplacerFixture().CreateReplacer();
                var torrent = Substitute.For<ITorrent>();
                torrent.Label.Returns("Misc/Other");
                torrent.Name.Returns("My test torrent");

                var parameters = new[]
                {
                    new Parameter {Pattern = "^(?<category>.*)/.*$", Source = ParameterSource.Label},
                    new Parameter {Pattern = "^.*/(?<subcat>.*)$", Source = ParameterSource.Label},
                    new Parameter {Pattern = "^My (?<name>.*) torrent$", Source = ParameterSource.Name}
                };

                // When
                var result = replacer.Replace(torrent, parameters, "//${category}/${subcat}/${name}");

                // Then
                Assert.Equal("//Misc/Other/test", result);
            }
        }
    }
}
