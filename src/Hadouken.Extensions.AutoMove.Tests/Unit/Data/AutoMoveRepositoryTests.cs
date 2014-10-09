using System;
using System.Linq;
using Hadouken.Extensions.AutoMove.Data;
using Hadouken.Extensions.AutoMove.Data.Models;
using Hadouken.Extensions.AutoMove.Tests.Fixtures;
using Xunit;

namespace Hadouken.Extensions.AutoMove.Tests.Unit.Data
{
    public sealed class AutoMoveRepositoryTests
    {
        public sealed class TheConstructor
        {
            [Fact]
            public void Should_Throw_Exception_If_Connection_Is_Null()
            {
                // Given, When
                var exception = Record.Exception(() => new AutoMoveRepository(null));

                // Then
                Assert.IsType<ArgumentNullException>(exception);
                Assert.Equal("dbConnection", ((ArgumentNullException) exception).ParamName);
            }
        }

        public sealed class TheCreateRuleMethod
        {
            [Fact]
            public void Should_Insert_Rule()
            {
                // Given
                var repository = new AutoMoveRepositoryFixture().CreateRepository();

                // When
                repository.CreateRule(new Rule {Name = "Rule name", TargetPath = "//path"});

                // Then
                Assert.Equal(1, repository.GetRules().Count());
            }
        }

        public sealed class TheCreateParameterMethod
        {
            [Fact]
            public void Should_Insert_Parameter()
            {
                // Given
                var repository = new AutoMoveRepositoryFixture().CreateRepository();

                // When
                repository.CreateParameter(new Parameter {RuleId = 1, Pattern = ".*"});

                // Then
                Assert.Equal(1, repository.GetParametersByRuleId(1).Count());
            }
        }

        public sealed class TheDeleteRuleMethod
        {
            [Fact]
            public void Should_Delete_Rule()
            {
                // Given
                var repository = new AutoMoveRepositoryFixture().CreateRepository();
                repository.CreateRule(new Rule {Name = "Rule name", TargetPath = "//path"});

                // When
                repository.DeleteRule(1);

                // Then
                Assert.Equal(0, repository.GetRules().Count());
            }
        }

        public sealed class TheDeleteParameterMethod
        {
            [Fact]
            public void Should_Delete_Parameter()
            {
                // Given
                var repository = new AutoMoveRepositoryFixture().CreateRepository();
                repository.CreateParameter(new Parameter {RuleId = 1, Pattern = ".*"});

                // When
                repository.DeleteParameter(1);

                // Then
                Assert.Equal(0, repository.GetParametersByRuleId(1).Count());
            }
        }

        public sealed class TheGetRulesMethod
        {
            [Fact]
            public void Should_Return_Empty_Enumerable_If_No_Rules_Exist()
            {
                // Given
                var repository = new AutoMoveRepositoryFixture().CreateRepository();

                // When
                var rules = repository.GetRules();

                // Then
                Assert.Equal(0, rules.Count());
            }

            [Fact]
            public void Should_Return_Created_Rules()
            {
                // Given
                var repository = new AutoMoveRepositoryFixture().CreateRepository();
                repository.CreateRule(new Rule {Name = "Rule #1", TargetPath = "//Path/1"});
                repository.CreateRule(new Rule {Name = "Rule #2", TargetPath = "//Path/2"});

                // When
                var rules = repository.GetRules();

                // Then
                Assert.Equal(2, rules.Count());
            }
        }

        public sealed class TheGetParametersByRuleIdMethod
        {
            [Fact]
            public void Should_Return_Empty_Enumerable_For_Invalid_Rule_Id()
            {
                // Given
                var repository = new AutoMoveRepositoryFixture().CreateRepository();

                // When
                var parameters = repository.GetParametersByRuleId(1);

                // Then
                Assert.Equal(0, parameters.Count());
            }

            [Fact]
            public void Should_Return_Enumerable_Of_Parameters_For_Valid_Rule_Id()
            {
                // Given
                var repository = new AutoMoveRepositoryFixture().CreateRepository();
                repository.CreateParameter(new Parameter {RuleId = 1, Pattern = ".*", Source = ParameterSource.Label});
                repository.CreateParameter(new Parameter {RuleId = 1, Pattern = ".*", Source = ParameterSource.Name});

                // When
                var parameters = repository.GetParametersByRuleId(1).ToList();

                // Then
                Assert.Equal(2, parameters.Count());
                Assert.Equal(ParameterSource.Label, parameters.First().Source);
                Assert.Equal(ParameterSource.Name, parameters.Last().Source);
            }
        }

        public sealed class TheUpdateRuleMethod
        {
            [Fact]
            public void Should_Not_Fail_On_Invalid_Rule_Id()
            {
                // Given
                var repository = new AutoMoveRepositoryFixture().CreateRepository();
                var rule = new Rule {Id = 123, Name = "Name", TargetPath = "Path"};

                // When
                var exception = Record.Exception(() => repository.UpdateRule(rule));

                // Then
                Assert.Null(exception);
            }

            [Fact]
            public void Should_Update_Valid_Rule()
            {
                // Given
                var repository = new AutoMoveRepositoryFixture().CreateRepository();
                repository.CreateRule(new Rule {Name = "Name", TargetPath = "Path"});

                // When
                repository.UpdateRule(new Rule {Id = 1, Name = "Updated name", TargetPath = "Path"});

                // Then
                Assert.Equal("Updated name", repository.GetRules().First().Name);
            }
        }

        public sealed class TheUpdateParameterMethod
        {
            [Fact]
            public void Should_Not_Fail_On_Invalid_Parameter_Id()
            {
                // Given
                var repository = new AutoMoveRepositoryFixture().CreateRepository();
                var parameter = new Parameter {Id = 123, RuleId = 1, Pattern = ".*"};

                // When
                var exception = Record.Exception(() => repository.UpdateParameter(parameter));

                // Then
                Assert.Null(exception);
            }

            [Fact]
            public void Should_Update_Valid_Parameter()
            {
                // Given
                var repository = new AutoMoveRepositoryFixture().CreateRepository();
                repository.CreateParameter(new Parameter {RuleId = 1, Pattern = ".*"});

                // When
                repository.UpdateParameter(new Parameter {Id = 1, RuleId = 1, Pattern = ".* updated"});

                // Then
                Assert.Equal(".* updated", repository.GetParametersByRuleId(1).First().Pattern);
            }
        }
    }
}
