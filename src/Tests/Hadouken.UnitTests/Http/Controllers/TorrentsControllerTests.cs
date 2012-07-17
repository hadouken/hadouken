using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Moq;
using Hadouken.BitTorrent;
using Hadouken.Impl.Http.Controllers.Api;
using Hadouken.Http;
using System.IO;
using JsonFx.Json;

namespace Hadouken.UnitTests.Http.Controllers
{
    [TestFixture]
    public class TorrentsControllerTests
    {
        [Test]
        public void Renders_list_as_null_without_torrents()
        {
            var bitEngine = new Mock<IBitTorrentEngine>();
            bitEngine.Setup(b => b.Managers).Returns(new Dictionary<string, ITorrentManager>());

            var ms = new MemoryStream();
            var context = CreateMockContextWithStream(ms);

            var controller = new TorrentsController(bitEngine.Object);
            
            var result = controller.List();
            result.Execute(context.Object);

            string data = Encoding.UTF8.GetString(ms.ToArray());

            Assert.IsTrue(data == "[]"); // empty json array
        }

        [Test]
        public void Renders_list_with_torrents()
        {
            var bitEngine = new Mock<IBitTorrentEngine>();
            
            var manager = new Mock<ITorrentManager>();
            manager.Setup(m => m.InfoHash).Returns("test");
            manager.Setup(m => m.Torrent).Returns(new Mock<ITorrent>().Object);

            var mgrs = new Dictionary<string, ITorrentManager>();
            mgrs.Add("test", manager.Object);

            bitEngine.Setup(b => b.Managers).Returns(mgrs);

            var ms = new MemoryStream();
            var context = CreateMockContextWithStream(ms);

            var controller = new TorrentsController(bitEngine.Object);

            var result = controller.List();
            result.Execute(context.Object);

            string data = Encoding.UTF8.GetString(ms.ToArray());

            List<Dictionary<string,string>> json = new JsonReader().Read<List<Dictionary<string,string>>>(data);

            Assert.IsTrue(json.Count == 1);
            Assert.IsTrue(json[0]["InfoHash"] == "test");
        }

        private static Mock<IHttpContext> CreateMockContextWithStream(Stream s)
        {
            var context = new Mock<IHttpContext>();
            var response = new Mock<IHttpResponse>();

            context.Setup(c => c.Response).Returns(response.Object);
            response.Setup(r => r.OutputStream).Returns(s);

            return context;
        }
    }
}
