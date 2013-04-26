using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hadouken.BitTorrent;
using Hadouken.Common.BitTorrent;
using Moq;
using NUnit.Framework;
using Hadouken.Impl.BitTorrent.Handlers;

namespace Hadouken.UnitTests.BitTorrent.Handlers
{
    [TestFixture]
    public class AddTorrentHandlerTests
    {
        [Test]
        public void Can_add_torrents()
        {
            // Setup
            var torrent = TestHelper.LoadResource("Hadouken.UnitTests.Resources.ubuntu.torrent");
            var bt = new Mock<IBitTorrentEngine>();

            // Test
            var handler = new AddTorrentHandler(bt.Object);
            handler.Handle(new AddTorrentMessage()
                {
                    Data = torrent.ToArray(),
                });

            // Verify
            bt.Verify(b => b.AddTorrent(It.IsAny<byte[]>()), Times.Once());
        }
    }
}
