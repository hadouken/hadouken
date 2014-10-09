using System;
using Hadouken.Common.BitTorrent;
using Hadouken.Extensions.AutoMove.Data;
using Hadouken.Extensions.AutoMove.Data.Models;
using Hadouken.Extensions.AutoMove.Tests.Fixtures;
using NSubstitute;
using Xunit;

namespace Hadouken.Extensions.AutoMove.Tests.Unit
{
    public sealed class RuleFinderTests
    {
        public sealed class TheConstructor
        {
            [Fact]
            public void Should_Throw_Exception_If_Repository_Is_Null()
            {
                // Given, When
                var exception = Record.Exception(() => new RuleFinder(null, Substitute.For<ISourceValueProvider>()));

                // Then
                Assert.IsType<ArgumentNullException>(exception);
                Assert.Equal("autoMoveRepository", ((ArgumentNullException) exception).ParamName);
            }

            [Fact]
            public void Should_Throw_Exception_If_Source_Value_Provider_Is_Null()
            {
                // Given, When
                var exception = Record.Exception(() => new RuleFinder(Substitute.For<IAutoMoveRepository>(), null));

                // Then
                Assert.IsType<ArgumentNullException>(exception);
                Assert.Equal("sourceValueProvider", ((ArgumentNullException) exception).ParamName);
            }
        }

        public sealed class TheFindRuleMethod
        {
            [Fact]
            public void Should_Return_Null_If_No_Rule_Was_Found()
            {
                // Given
                var ruleFinder = new RuleFinderFixture().CreateRuleFinder();
                var torrent = Substitute.For<ITorrent>();

                // When
                var rule = ruleFinder.FindRule(torrent);

                // Then
                Assert.Null(rule);
            }

            [Fact]
            public void Should_Return_Null_If_One_Rule_Exist_And_Does_Not_Match_Torrent()
            {
                // Given
                var fixture = new RuleFinderFixture();
                fixture.AutoMoveRepository.CreateRule(new Rule {Name = "Test rule", TargetPath = "//target-path"});
                fixture.AutoMoveRepository.CreateParameter(new Parameter {Pattern = "^My label$", RuleId = 1, Source = ParameterSource.Label});

                var torrent = Substitute.For<ITorrent>();
                torrent.Label.Returns("Other label");

                var ruleFinder = fixture.CreateRuleFinder();

                // When
                var rule = ruleFinder.FindRule(torrent);

                // Then
                Assert.Null(rule);
            }

            [Fact]
            public void Should_Return_Correct_Rule_If_One_Rule_Exist_That_Matches_Torrent()
            {
                // Given
                var fixture = new RuleFinderFixture();
                fixture.AutoMoveRepository.CreateRule(new Rule {Name = "Test rule", TargetPath = "//target-path"});
                fixture.AutoMoveRepository.CreateParameter(new Parameter {Pattern = "^My label$", RuleId = 1, Source = ParameterSource.Label});

                var torrent = Substitute.For<ITorrent>();
                torrent.Label.Returns("My label");

                var ruleFinder = fixture.CreateRuleFinder();

                // When
                var rule = ruleFinder.FindRule(torrent);

                // Then
                Assert.NotNull(rule);
                Assert.Equal("Test rule", rule.Name);
            }

            [Fact]
            public void Should_Return_Null_If_One_Rule_With_Multiple_Parameters_Does_Not_Match_Torrent()
            {
                // Given
                var fixture = new RuleFinderFixture();
                fixture.AutoMoveRepository.CreateRule(new Rule {Name = "Test rule", TargetPath = "//target-path"});
                fixture.AutoMoveRepository.CreateParameter(new Parameter {Pattern = "^My label$", RuleId = 1, Source = ParameterSource.Label});
                fixture.AutoMoveRepository.CreateParameter(new Parameter {Pattern = "^Torrent name$", RuleId = 1, Source = ParameterSource.Name});

                var torrent = Substitute.For<ITorrent>();
                torrent.Label.Returns("My label");
                torrent.Name.Returns("Other name");

                var ruleFinder = fixture.CreateRuleFinder();

                // When
                var rule = ruleFinder.FindRule(torrent);

                // Then
                Assert.Null(rule);
            }

            [Fact]
            public void Should_Return_Rule_If_One_Rule_With_Multiple_Parameters_Matches_Torrent()
            {
                // Given
                var fixture = new RuleFinderFixture();
                fixture.AutoMoveRepository.CreateRule(new Rule {Name = "Test rule", TargetPath = "//target-path"});
                fixture.AutoMoveRepository.CreateParameter(new Parameter {Pattern = "^My label$", RuleId = 1, Source = ParameterSource.Label});
                fixture.AutoMoveRepository.CreateParameter(new Parameter {Pattern = "^Torrent name$", RuleId = 1, Source = ParameterSource.Name});

                var torrent = Substitute.For<ITorrent>();
                torrent.Label.Returns("My label");
                torrent.Name.Returns("Torrent name");

                var ruleFinder = fixture.CreateRuleFinder();

                // When
                var rule = ruleFinder.FindRule(torrent);

                // Then
                Assert.NotNull(rule);
                Assert.Equal("Test rule", rule.Name);
            }

            [Fact]
            public void Should_Return_First_Rule_If_Multiple_Rules_Matches_Torrent()
            {
                // Given
                var fixture = new RuleFinderFixture();
                fixture.AutoMoveRepository.CreateRule(new Rule {Name = "Test rule #1", TargetPath = "//target-path"});
                fixture.AutoMoveRepository.CreateParameter(new Parameter {Pattern = "^My label$", RuleId = 1, Source = ParameterSource.Label});
                fixture.AutoMoveRepository.CreateRule(new Rule {Name = "Test rule #2", TargetPath = "//target-path"});
                fixture.AutoMoveRepository.CreateParameter(new Parameter {Pattern = "^Torrent name$", RuleId = 2, Source = ParameterSource.Name});

                var torrent = Substitute.For<ITorrent>();
                torrent.Label.Returns("My label");
                torrent.Name.Returns("Torrent name");

                var ruleFinder = fixture.CreateRuleFinder();

                // When
                var rule = ruleFinder.FindRule(torrent);

                // Then
                Assert.NotNull(rule);
                Assert.Equal("Test rule #1", rule.Name);
            }
        }
    }
}
