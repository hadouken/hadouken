using Hadouken.Fx.JsonRpc;
using NUnit.Framework;

namespace Hadouken.Fx.Tests.JsonRpc
{
    [TestFixture]
    public class JsonRpcRequestParserTests
    {
        [Test]
        public void Parse_WithValidJson_ReturnsRequestObject()
        {
            // Given
            var json = "{ \"id\": 1, \"jsonrpc\": \"2.0\", \"method\": \"foo\", \"params\": 1 }";
            var parser = new JsonRpcRequestParser(new JsonSerializer());

            // When
            var result = parser.Parse(json);

            // Then
            Assert.AreEqual(1, result.Id);
            Assert.AreEqual("2.0", result.ProtocolVersion);
            Assert.AreEqual("foo", result.MethodName);
            Assert.AreEqual(1, result.Parameters);
        }
    }
}
