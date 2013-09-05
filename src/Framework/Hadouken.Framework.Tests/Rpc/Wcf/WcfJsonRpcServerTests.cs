using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hadouken.Framework.Rpc;
using Hadouken.Framework.Rpc.Wcf;
using Moq;
using NUnit.Framework;

namespace Hadouken.Framework.Tests.Rpc.Wcf
{
    public class WcfJsonRpcServerTests
    {
        [Test]
        public void StartStop_DoesNotThrow()
        {
            var requestBuilder = new Mock<IRequestBuilder>();
            var requestHandler = new Mock<IRequestHandler>();

            var server = new WcfJsonRpcServer("test", requestBuilder.Object, requestHandler.Object);

            Assert.DoesNotThrow(() =>
            {
                server.Start();
                server.Stop();
            });
        }

        [Test]
        public void Call_WithValidRequest_ReturnsValidResponse()
        {
            var server = new WcfJsonRpcServer("test", new RequestBuilder(),
                new RequestHandler(new[] {new StringReverser()}));
            server.Start();

            var client = new JsonRpcClient(new WcfClientTransport("net.pipe://localhost/test"));
            var result = client.Call<string>("stringreverser", "foobar").Result;

            Assert.AreEqual("raboof", result);
        }
    }
}
