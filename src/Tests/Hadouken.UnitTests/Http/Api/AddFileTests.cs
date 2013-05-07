using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hadouken.Http.Api;
using NUnit.Framework;
using Moq;
using Hadouken.BitTorrent;
using System.IO;
using Hadouken.Http;

namespace Hadouken.UnitTests.Http.Api
{
    [TestFixture]
    public class AddFileTests
    {
        [Test]
        public void Can_add_torrents()
        {
            var torrent = TestHelper.LoadResource("Hadouken.UnitTests.Resources.ubuntu.torrent");
            var manager = new Mock<ITorrentManager>();
            var bt = new Mock<IBitTorrentEngine>();
            var rs = new Mock<IHttpResponse>();
            var ctx = HttpContextHelper.CreateMockWithPostedFiles(torrent);

            // Test setup
            manager.SetupGet(m => m.InfoHash).Returns("abcd");
            bt.Setup(b => b.AddTorrent(It.IsAny<byte[]>())).Returns(manager.Object);
            ctx.SetupGet(c => c.Response).Returns(rs.Object);

            // Run code
            var aft = new AddFile(bt.Object);
            aft.Context = ctx.Object;
            var result = aft.Execute();

            // Verify
            bt.Verify(b => b.AddTorrent(It.IsAny<byte[]>()), Times.Once());

            Assert.IsInstanceOf<RedirectResult>(result);

            // Execute the result
            result = (RedirectResult)result;
            result.Execute(aft.Context);

            rs.Verify(r => r.Redirect("/api?action=gettorrents&hash=abcd"), Times.Once());
        }
    }
}
