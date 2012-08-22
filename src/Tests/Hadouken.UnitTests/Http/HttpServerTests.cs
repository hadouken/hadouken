using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Hadouken.Impl.Http;
using Moq;
using Hadouken.Data;
using Hadouken.IO;
using System.Net;
using System.IO;
using WatiN.Core;
using Hadouken.Configuration;

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
