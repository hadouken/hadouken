using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hadouken.BitTorrent;
using Hadouken.Http;
using Hadouken.Http.Api;
using Hadouken.Impl.BitTorrent;
using Moq;
using NUnit.Framework;

namespace Hadouken.UnitTests.Http.Api
{
    [TestFixture]
    public class GetTorrentsTests
    {
        [Test]
        public void Empty_list_when_no_torrents()
        {
            var engine = new Mock<IBitTorrentEngine>();
            engine.SetupGet(e => e.Managers).Returns(() => null);

            var action = new GetTorrents(engine.Object);
            var result = action.Execute();

            Assert.IsTrue(result != null);
            Assert.IsInstanceOf(typeof(JsonResult), result);
        }

        [Test]
        public void Returns_valid_object_when_torrents()
        {
            var engine = new Mock<IBitTorrentEngine>();
            var manager = new Mock<ITorrentManager>();

            engine.SetupGet(e => e.Managers).Returns(new Dictionary<string, ITorrentManager>()
                                                         {{"abcd", manager.Object}});

            var action = new GetTorrents(engine.Object);
            var result = action.Execute();

            Assert.IsTrue(result != null);
            Assert.IsInstanceOf(typeof(JsonResult), result);
        }
    }
}
