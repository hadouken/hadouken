using System;
using Hadouken.Framework.Rpc;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace Hadouken.Framework.Tests.Rpc
{
    public class TestService : IJsonRpcService
    {
        [JsonRpcMethod("test")]
        public string Test()
        {
            return "test";
        }

        [JsonRpcMethod("test.exception")]
        public string TestException()
        {
            throw new Exception();
        }
    }

    public class RequestHandlerTests
    {
        [Test]
        public void CreateNewRequestHandlerBuildsServiceCache()
        {
            var handler = new RequestHandler(new IJsonRpcService[] {new TestService()});
            Assert.AreEqual(2, handler.Services.Count);
        }

        [Test]
        public void ExecuteWithNoParams()
        {
            var handler = new RequestHandler(new IJsonRpcService[] {new TestService()});
            var response = (JsonRpcSuccessResponse)handler.Execute(new JsonRpcRequest {Id = 1, Method = "test"});
            Assert.AreEqual("test", response.Result.ToString());
        }

        [Test]
        public void ExecuteWithMissingMethodReturnsError()
        {
            var handler = new RequestHandler(new IJsonRpcService[] {new TestService()});
            var response = (JsonRpcErrorResponse) handler.Execute(new JsonRpcRequest {Id = 1, Method = "missing"});
            Assert.AreEqual(JsonRpcErrorResponse.MethodNotFound(-1, "").Error.ErrorCode, response.Error.ErrorCode);
        }

        [Test]
        public void ExecuteWithInvalidParametersReturnsError()
        {
            var handler = new RequestHandler(new IJsonRpcService[] {new TestService()});
            var response =
                (JsonRpcErrorResponse)
                    handler.Execute(new JsonRpcRequest {Id = 1, Method = "test", Parameters = new JArray(1, 2, 3)});
            Assert.AreEqual(JsonRpcErrorResponse.InvalidParams(-1).Error.ErrorCode, response.Error.ErrorCode);
        }

        [Test]
        public void ExecuteWithMethodThatTrowsExceptionReturnsError()
        {
            var handler = new RequestHandler(new IJsonRpcService[] {new TestService()});
            var response =
                (JsonRpcErrorResponse) handler.Execute(new JsonRpcRequest {Id = 1, Method = "test.exception"});
            Assert.AreEqual(JsonRpcErrorResponse.InternalRpcError(-1).Error.ErrorCode, response.Error.ErrorCode);
        }
    }
}
