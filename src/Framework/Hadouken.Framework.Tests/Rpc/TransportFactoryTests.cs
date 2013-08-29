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

        [Test]
        public void CreateTransport_WithNoScheme_ReturnsNull()
        {
            var transportFactory = new TransportFactory(null);

            var transport = transportFactory.CreateTransport("//no-scheme.com/");

            Assert.IsNull(transport);
        }

        [Test]
        public void CreateTransport_WithNoRegisteredTransport_ReturnsNull()
        {
            var fakeTransport = new Mock<ITransport>();
            fakeTransport.Setup(t => t.SupportsScheme(It.IsAny<string>())).Returns(false);

            var transportFactory = new TransportFactory(new[] {fakeTransport.Object});
            var transport = transportFactory.CreateTransport("http://test/");

            Assert.IsNull(transport);
        }
    }
}
