using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;

using Hadouken.Configuration;
using Hadouken.Data;
using Hadouken.Impl.Http;
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

            var server = new DefaultHttpServer(kvs.Object, fs.Object);
            server.Start();
            server.Stop();
        }
    }
}
