using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hadouken.Framework.Rpc;
using NUnit.Framework;

namespace Hadouken.Framework.Tests.Rpc
{
    public class RequestHandlerTests
    {
        [Test]
        public void Execute_WithValidRequest_ReturnsCorrectValue()
        {
            var request = new Request { Method = "stringreverser", ParameterAsJson = "\"foo\"" };
            var requestHandler = new RequestHandler(new[] {new StringReverser() });

            var response = requestHandler.Execute(request) as SuccessResponse;
            var result = response.Result.ToString();

            Assert.AreEqual("oof", result);
        }

        [Test]
        public void Execute_WithMissingMethod_ReturnsMethodDoesNotExistError()
        {
            var request = new Request { Method = "missingmethod" };
            var requestHandler = new RequestHandler(new[] { new StringReverser() });

            var response = requestHandler.Execute(request) as ErrorResponse;
            
            Assert.AreEqual(typeof(MethodDoesNotExistError), response.Error.GetType());
        }

        [Test]
        public void Execute_WithNullRequest_ReturnsParseError()
        {
            var requestHandler = new RequestHandler(new[] { new StringReverser() });

            var response = requestHandler.Execute(null) as ErrorResponse;

            Assert.AreEqual(typeof(ParseError), response.Error.GetType());
        }
    }

    [RpcMethod("stringreverser")]
    public class StringReverser : IRpcMethod
    {
        public string Execute(string param)
        {
            return new string(param.Reverse().ToArray());
        }
    }
}
