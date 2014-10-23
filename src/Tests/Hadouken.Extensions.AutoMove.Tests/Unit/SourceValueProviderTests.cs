using Hadouken.Common.BitTorrent;
using Hadouken.Extensions.AutoMove.Data.Models;
using NSubstitute;
using Xunit;

namespace Hadouken.Extensions.AutoMove.Tests.Unit
{
    public sealed class SourceValueProviderTests
    {
        public sealed class TheGetValueMethod
        {
            [Fact]
            public void Should_Return_Torrent_Label_If_Source_Is_Label()
            {
                // Given
                var provider = new SourceValueProvider();
                var torrent = Substitute.For<ITorrent>();
                torrent.Label.Returns("Test label");

                // When
                var value = provider.GetValue(torrent, ParameterSource.Label);

                // Then
                Assert.Equal("Test label", value);
            }

            [Fact]
            public void Should_Return_Torrent_Name_If_Source_Is_Name()
            {
                // Given
                var provider = new SourceValueProvider();
                var torrent = Substitute.For<ITorrent>();
                torrent.Name.Returns("Test name");

                // When
                var value = provider.GetValue(torrent, ParameterSource.Name);

                // Then
                Assert.Equal("Test name", value);
            }
        }
    }
}
