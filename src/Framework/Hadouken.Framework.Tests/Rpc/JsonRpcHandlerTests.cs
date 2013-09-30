using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hadouken.Framework.Rpc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace Hadouken.Framework.Tests.Rpc
{
    public sealed class CalculatorService : IJsonRpcService
    {
        [JsonRpcMethod("add")]
        public int Add(int i1, int i2)
        {
            return i1 + i2;
        }
    }

    public sealed class EchoService : IJsonRpcService
    {
        [JsonRpcMethod("echo")]
        public string Echo(string input)
        {
            return new string(input.Reverse().ToArray());
        }
    }

    public sealed class ObjectService : IJsonRpcService
    {
        [JsonRpcMethod("object.input")]
        public bool ObjectInput(ObjDto dto)
        {
            return dto.Test == "foobar";
        }
    }

    public sealed class ObjDto
    {
        public string Test { get; set; }
    }

    public class JsonRpcHandlerTests
    {
        [Test]
        public void HandleAsync_WithSingleParameterCall_Succeeds()
        {
            var data = GenerateRequest(new {id = 1, method = "echo", @params = "foobar", jsonrpc = "2.0"});
            var handler = new JsonRpcHandler(new RequestHandler(new[] {new EchoService()}));

            var result = GetResponseData<string>(handler.HandleAsync(data).Result);

            Assert.AreEqual("raboof", result);
        }

        [Test]
        public void HandleAsync_WithTwoParametersMethodCall_ReturnsExpectedResult()
        {
            var data = GenerateRequest(new {id = 1, method = "add", @params = new[] {2, 2}, jsonrpc = "2.0"});
            var handler = new JsonRpcHandler(new RequestHandler(new[] {new CalculatorService()}));

            var result = GetResponseData<int>(handler.HandleAsync(data).Result);

            Assert.AreEqual(4, result);
        }

        [Test]
        public void HandleAsync_WithObjectParameterCall_Succeeds()
        {
            var data =
                GenerateRequest(new {id = 1, method = "object.input", @params = new {Test = "foobar"}, jsonrpc = "2.0"});
            var handler = new JsonRpcHandler(new RequestHandler(new[] {new ObjectService()}));

            var result = GetResponseData<bool>(handler.HandleAsync(data).Result);

            Assert.AreEqual(true, result);
        }

        #region JSON helper methods

        private static string GenerateRequest(object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        private static T GetResponseData<T>(string json)
        {
            var jsonObject = JObject.Parse(json);
            return jsonObject["result"].Value<T>();
        }

        #endregion
    }
}
