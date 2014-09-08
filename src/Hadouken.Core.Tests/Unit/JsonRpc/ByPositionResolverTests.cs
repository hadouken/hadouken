using System;
using System.Collections.Generic;
using Hadouken.Common.Text;
using Hadouken.Core.JsonRpc;
using Hadouken.Core.Tests.Fakes;
using Xunit;

namespace Hadouken.Core.Tests.Unit.JsonRpc
{
    public class ByPositionResolverTests
    {
        public class TheConstructor
        {
            [Fact]
            public void Throws_ArgumentNullException_If_Serializer_Is_Null()
            {
                // Given, When
                var exception = Record.Exception(() => new ByPositionResolver(null));

                // Then
                Assert.IsType<ArgumentNullException>(exception);
                Assert.Equal("serializer", ((ArgumentNullException) exception).ParamName);
            }
        }

        public class TheCanResolveMethod
        {
            [Fact]
            public void Returns_True_For_Array_Of_Objects()
            {
                // Given
                var resolver = CreateResolver();

                // When
                var result = resolver.CanResolve(new object[] { 1, "foo", 3 });

                // Then
                Assert.True(result);
            }

            [Fact]
            public void Returns_False_For_Simple_Value()
            {
                // Given
                var resolver = CreateResolver();

                // When
                var result = resolver.CanResolve(1);

                // Then
                Assert.False(result);
            }

            [Fact]
            public void Returns_False_For_String()
            {
                // Given
                var resolver = CreateResolver();

                // When
                var result = resolver.CanResolve("test");

                // Then
                Assert.False(result);
            }

            [Fact]
            public void Returns_True_For_Generic_List_Of_Strings()
            {
                // Given
                var parameter = new List<string>() { "One", "Two" };
                var resolver = CreateResolver();

                // When
                var result = resolver.CanResolve(parameter);

                // Then
                Assert.True(result);
            }

            [Fact]
            public void Returns_False_For_Dictionary()
            {
                // Given
                var resolver = CreateResolver();

                // When
                var result = resolver.CanResolve(new Dictionary<string, object>());

                // Then
                Assert.False(result);
            }

            [Fact]
            public void Returns_False_For_Dictionary_Of_Lists()
            {
                // Given
                var resolver = CreateResolver();

                // When
                var result = resolver.CanResolve(new Dictionary<string, List<string>>());

                // Then
                Assert.False(result);
            }
        }

        public class TheResolveMethod
        {
            [Fact]
            public void Throws_ParameterLengthMismatchException_If_Parameter_Lengths_Differ()
            {
                // Given
                var request = new object[] { 1, "foo", 2 };
                var target = new IParameter[] { new ParameterFake("test", typeof(int)) };
                var resolver = CreateResolver();

                // When
                var exception = Record.Exception(() => resolver.Resolve(request, target));

                // Then
                Assert.IsType<ParameterLengthMismatchException>(exception);
            }

            [Fact]
            public void Returns_Correct_Result_Length_For_Valid_Request_Parameters()
            {
                // Given
                var request = new object[] { 1, 2, "foo" };
                var parameters = new IParameter[]
                {
                    new ParameterFake("1", typeof (int)),
                    new ParameterFake("2", typeof (int)),
                    new ParameterFake("3", typeof (string))
                };
                var resolver = CreateResolver();

                // When
                var result = resolver.Resolve(request, parameters);

                // Then
                Assert.Equal(3, result.Length);
            }
        }

        private static ByPositionResolver CreateResolver()
        {
            return new ByPositionResolver(new JsonSerializer());
        }
    }
}
