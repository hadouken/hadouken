using System;
using Hadouken.Extensions.AutoMove.Data.Models;
using Hadouken.Extensions.AutoMove.Services;
using Hadouken.Extensions.AutoMove.Tests.Fixtures;
using NSubstitute;
using Xunit;

namespace Hadouken.Extensions.AutoMove.Tests.Unit {
    public class AutoMoveServiceTests {
        public sealed class TheConstructor {
            [Fact]
            public void Should_Throw_Exception_If_Auto_Move_Repository_Is_Null() {
                // Given, When
                var exception = Record.Exception(() => new AutoMoveService(null));

                // Then
                Assert.IsType<ArgumentNullException>(exception);
                Assert.Equal("autoMoveRepository", ((ArgumentNullException) exception).ParamName);
            }
        }

        public sealed class TheCreateRuleMethod {
            [Fact]
            public void Should_Throw_Exception_If_Rule_Is_Null() {
                // Given
                var service = new AutoMoveServiceFixture().CreateService();

                // When
                var exception = Record.Exception(() => service.CreateRule(null));

                // Then
                Assert.IsType<ArgumentNullException>(exception);
                Assert.Equal("rule", ((ArgumentNullException) exception).ParamName);
            }

            [Fact]
            public void Should_Call_Create_Rule_On_Repository() {
                // Given
                var fixture = new AutoMoveServiceFixture();
                var service = fixture.CreateService();

                // When
                service.CreateRule(new Rule());

                // Then
                fixture.AutoMoveRepository.Received(1).CreateRule(Arg.Any<Rule>());
            }

            [Fact]
            public void Should_Return_The_Rule_Passed_As_Argument() {
                // Given
                var service = new AutoMoveServiceFixture().CreateService();

                // When
                var rule = service.CreateRule(new Rule {Name = "<name>"});

                // Then
                Assert.Equal("<name>", rule.Name);
            }
        }

        public sealed class TheCreateParameterMethod {
            [Fact]
            public void Should_Throw_If_Parameter_Is_Null() {
                // Given
                var service = new AutoMoveServiceFixture().CreateService();

                // When
                var exception = Record.Exception(() => service.CreateParameter(null));

                // Then
                Assert.IsType<ArgumentNullException>(exception);
                Assert.Equal("parameter", ((ArgumentNullException) exception).ParamName);
            }
        }
    }
}