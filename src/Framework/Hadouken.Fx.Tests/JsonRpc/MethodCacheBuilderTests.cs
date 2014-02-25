using System.Linq;
using Hadouken.Fx.JsonRpc;
using NUnit.Framework;

namespace Hadouken.Fx.Tests.JsonRpc
{
    public class MethodCacheBuilderTests
    {
        [Test]
        public void Build_WithNoServices_ReturnsEmptyCache()
        {
            // Given
            var builder = new MethodCacheBuilder(Enumerable.Empty<IJsonRpcService>());

            // When
            var result = builder.Build();

            // Then
            Assert.IsFalse(result.GetAll().Any());
        }

        [Test]
        public void Build_WithOneService_ReturnsCorrectlyBuiltCache()
        {
            // Given
            var builder = new MethodCacheBuilder(new[] {new TestService()});

            // When
            var result = builder.Build();

            // Then
            Assert.IsNotNull(result.Get("int32.noParams"));
        }
    }
}