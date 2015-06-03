using System;
using System.Collections;
using System.Collections.Generic;
using Hadouken.Common.Text;
using Hadouken.Core.JsonRpc;
using Hadouken.Core.Tests.Fakes;
using Xunit;

namespace Hadouken.Core.Tests.Unit.JsonRpc {
    public class ByNameResolverTests {
        private static ByNameResolver CreateResolver() {
            return new ByNameResolver(new JsonSerializer());
        }

        public class TheConstructor {
            [Fact]
            public void Throws_ArgumentNullException_If_Serializer_Is_Null() {
                // Given, When
                var exception = Record.Exception(() => new ByNameResolver(null));

                // Then
                Assert.IsType<ArgumentNullException>(exception);
                Assert.Equal("serializer", ((ArgumentNullException) exception).ParamName);
            }
        }

        public class TheCanResolveMethod {
            [Fact]
            public void Returns_True_For_Dictionary_Of_Correct_Type() {
                // Given
                var resolver = CreateResolver();

                // When
                var result = resolver.CanResolve(new Dictionary<string, object>());

                // Then
                Assert.True(result);
            }

            [Fact]
            public void Returns_False_For_Dictionary_Of_Wrong_Type() {
                // Given
                var resolver = CreateResolver();

                // When
                var result = resolver.CanResolve(new Dictionary<string, int>());

                // Then
                Assert.False(result);
            }

            [Fact]
            public void Returns_False_For_Hashtable() {
                // Given
                var resolver = CreateResolver();

                // When
                var result = resolver.CanResolve(new Hashtable());

                // Then
                Assert.False(result);
            }
        }

        public class TheResolveMethod {
            [Fact]
            public void Returns_Correct_Result_For_Dictionary_With_One_Pair() {
                // Given
                var request = new Dictionary<string, object> {{"test", 1}};
                var target = new IParameter[] {new ParameterFake("test", typeof (int))};
                var resolver = CreateResolver();

                // When
                var result = resolver.Resolve(request, target);

                // Then
                Assert.Equal(1, result.Length);
            }

            [Fact]
            public void Returns_Correct_Result_For_Dictionary_With_Multiple_Pairs() {
                // Given
                var input = new Dictionary<string, object> {{"test1", 1}, {"test2", new[] {1, 2, 3}}};
                var parameters = new IParameter[]
                {new ParameterFake("test1", typeof (int)), new ParameterFake("test2", typeof (int[]))};
                var resolver = CreateResolver();

                // When
                var result = resolver.Resolve(input, parameters);

                // Then
                Assert.Equal(2, result.Length);
            }

            [Fact]
            public void Throws_ParameterNameNotFoundException_If_Request_Has_Missing_Name() {
                // Given
                var input = new Dictionary<string, object> {{"missing", 1}, {"test2", new[] {1, 2, 3}}};
                var parameters = new IParameter[]
                {new ParameterFake("test1", typeof (int)), new ParameterFake("test2", typeof (int[]))};
                var resolver = CreateResolver();

                // When
                var exception = Record.Exception(() => resolver.Resolve(input, parameters));

                // Then
                Assert.IsType<ParameterNameNotFoundException>(exception);
            }

            [Fact]
            public void Throws_ParameterLengthMismatchException_If_Parameter_Lengths_Differ() {
                // Given
                var input = new Dictionary<string, object> {{"test1", 1}, {"test2", new[] {1, 2, 3}}};
                var parameters = new IParameter[] {new ParameterFake("test1", typeof (int))};
                var resolver = CreateResolver();

                // When
                var exception = Record.Exception(() => resolver.Resolve(input, parameters));

                // Then
                Assert.IsType<ParameterLengthMismatchException>(exception);
            }
        }
    }
}