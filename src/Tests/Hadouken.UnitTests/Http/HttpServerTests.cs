using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;

using Hadouken.Configuration;
using Hadouken.Data;
using Hadouken.Http;
using Hadouken.Http.HttpServer;
using Hadouken.IO;

using Moq;
using NUnit.Framework;

namespace Hadouken.UnitTests.Http
{
    [TestFixture]
    public class HttpServerTests
    {
        [TestFixtureSetUp]
        public void Setup()
        {
            HdknConfig.ConfigManager = new MemoryConfigManager();
        }

        [Test]
        public void Can_start_and_stop_HTTP_server()
        {
            var kvs = new Mock<IKeyValueStore>();
            var fs = new Mock<IFileSystem>();
            var rr = new Mock<IRegistryReader>();

            var server = new DefaultHttpServer(kvs.Object, rr.Object, fs.Object);
            server.Start();
            server.Stop();
        }
    }
}
