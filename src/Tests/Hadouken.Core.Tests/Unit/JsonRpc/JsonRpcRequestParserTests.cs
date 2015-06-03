using System;
using Hadouken.Common.Text;
using Hadouken.Core.JsonRpc;
using Xunit;

namespace Hadouken.Core.Tests.Unit.JsonRpc {
    public class JsonRpcRequestParserTests {
        public class TheConstructor {
            [Fact]
            public void Throws_ArgumentNullException_If_Serializer_Is_Null() {
                // Given, When
                var exception = Record.Exception(() => new JsonRpcRequestParser(null));

                // Then
                Assert.IsType<ArgumentNullException>(exception);
                Assert.Equal("serializer", ((ArgumentNullException) exception).ParamName);
            }
        }

        public class TheParseMethod {
            [Fact]
            public void Returns_Object_For_Valid_Json() {
                // Given
                const string json = "{ \"id\": 1, \"jsonrpc\": \"2.0\", \"method\": \"foo\", \"params\": 1 }";
                var parser = new JsonRpcRequestParser(new JsonSerializer());

                // When
                var result = parser.Parse(json);

                // Then
                Assert.Equal(1L, result.Id);
                Assert.Equal("2.0", result.ProtocolVersion);
                Assert.Equal("foo", result.MethodName);
                Assert.Equal(1L, result.Parameters);
            }

            [Fact]
            public void Throws_InvalidRequestException_For_Request_With_Invalid_Protocol_Version() {
                // Given
                const string json = "{\"jsonrpc\": \"1.0\", \"method\": \"test\", \"params\": \"bar\"}";
                var parser = new JsonRpcRequestParser(new JsonSerializer());

                // When
                var exception = Record.Exception(() => parser.Parse(json));

                // Then
                Assert.IsType<InvalidRequestException>(exception);
                Assert.Equal("Invalid protocol version.", exception.Message);
            }

            [Fact]
            public void Throws_InvalidRequestException_For_Request_With_Invalid_Method_Type() {
                // Given
                const string json = "{\"jsonrpc\": \"2.0\", \"method\": 1, \"params\": \"bar\"}";
                var parser = new JsonRpcRequestParser(new JsonSerializer());

                // When
                var exception = Record.Exception(() => parser.Parse(json));

                // Then
                Assert.IsType<InvalidRequestException>(exception);
                Assert.Equal("Invalid method name.", exception.Message);
            }

            [Fact]
            public void Throws_InvalidRequestException_For_Request_With_Object_As_Id() {
                // Given
                const string json = "{\"jsonrpc\": \"2.0\", \"id\": {}, \"method\": \"test\", \"params\": \"bar\"}";
                var parser = new JsonRpcRequestParser(new JsonSerializer());

                // When
                var exception = Record.Exception(() => parser.Parse(json));

                // Then
                Assert.IsType<InvalidRequestException>(exception);
                Assert.Equal("Invalid id.", exception.Message);
            }
        }
    }
}