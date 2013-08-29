using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hadouken.Framework.Rpc;
using Moq;
using NUnit.Framework;

namespace Hadouken.Framework.Tests.Rpc
{
    public class TransportFactoryTests
    {
        [Test]
        public void CreateTransport_WithSupportedScheme_ReturnsInstance()
        {
            var fakeTransport = new Mock<ITransport>();
            fakeTransport.Setup(t => t.SupportsScheme("http")).Returns(true);

            var transportFactory = new TransportFactory(new[] {fakeTransport.Object});
            var transport = transportFactory.CreateTransport("http://+:8080/test/");

            Assert.IsNotNull(transport);
        }
    }
}
