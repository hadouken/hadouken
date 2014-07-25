using System.Linq;
using Hadouken.Common.JsonRpc;
using Hadouken.Core.JsonRpc;
using Hadouken.Core.Tests.Fakes;
using Xunit;

namespace Hadouken.Core.Tests.Unit.JsonRpc
{
    public class MethodCacheBuilderTests
    {
        public class TheConstructor
        {
            [Fact]
            public void Does_Not_Throw_If_Services_Is_Null()
            {
                // Given, When, Then
                Assert.DoesNotThrow(() => new MethodCacheBuilder(null));
            }
        }

        public class TheBuildMethod
        {
            [Fact]
            public void Returns_Empty_Cache_When_Given_No_Services()
            {
                // Given
                var builder = new MethodCacheBuilder(Enumerable.Empty<IJsonRpcService>());

                // When
                var result = builder.Build();

                // Then
                Assert.Equal(0, result.GetAll().Count());
            }

            [Fact]
            public void Returns_Cache_With_Services_When_Services_Is_Not_Empty()
            {
                // Given
                var builder = new MethodCacheBuilder(new[] { new JsonRpcServiceFake() });

                // When
                var result = builder.Build();

                // Then
                Assert.NotNull(result.Get("test"));
            }

            [Fact]
            public void Throws_MethodNameAlreadyRegistered_For_Duplicate_Method_Names()
            {
                // Given
                var builder = new MethodCacheBuilder(new[] { new JsonRpcServiceFake(), new JsonRpcServiceFake() });

                // When
                var exception = Record.Exception(() => builder.Build());

                // Then
                Assert.IsType<MethodNameAlreadyRegisteredException>(exception);
                Assert.Equal("test", ((MethodNameAlreadyRegisteredException) exception).MethodName);
            }

            [Fact]
            public void Returns_Null_For_Missing_Method()
            {
                // Given
                var builder = new MethodCacheBuilder(new[] { new JsonRpcServiceFake() });

                // When
                var result = builder.Build().Get("missing.method");

                // Then
                Assert.Null(result);
            }
        }
    }
}
