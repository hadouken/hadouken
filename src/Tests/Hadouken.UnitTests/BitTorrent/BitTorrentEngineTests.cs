using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;
using Hadouken.Impl.BitTorrent;
using Moq;
using Hadouken.IO;
using Hadouken.Messaging;
using Hadouken.Data;
using Hadouken.Data.Models;
using Hadouken.Configuration;

namespace Hadouken.UnitTests.BitTorrent
{
    [TestFixture]
    public class BitTorrentEngineTests
    {
        [Test]
        public void Does_not_care_about_null_data()
        {
            var fs = new Mock<IFileSystem>();
            var bus = new Mock<IMessageBus>();
            var repo = new Mock<IDataRepository>();
            var kvs = new Mock<IKeyValueStore>();

            repo.Setup(r => r.List<TorrentInfo>()).Returns(new List<TorrentInfo>());

            var engine = new MonoTorrentEngine(fs.Object, bus.Object, repo.Object, kvs.Object);
            engine.Load();

            var t = engine.AddTorrent(null);

            Assert.IsNull(t);
        }

        [Test]
        public void Does_not_care_about_garbage_data()
        {
            var fs = new Mock<IFileSystem>();
            var bus = new Mock<IMessageBus>();
            var repo = new Mock<IDataRepository>();
            var kvs = new Mock<IKeyValueStore>();

            repo.Setup(r => r.List<TorrentInfo>()).Returns(new List<TorrentInfo>());

            var engine = new MonoTorrentEngine(fs.Object, bus.Object, repo.Object, kvs.Object);
            engine.Load();

            var t = engine.AddTorrent(new byte[] { 6, 7, 1, 4, 5, 3, 3 }); // garbage data :)

            Assert.IsNull(t);
        }

        [Test]
        public void Can_load_and_unload_torrent()
        {
            var fs = new Mock<IFileSystem>();
            var bus = new Mock<IMessageBus>();
            var repo = new Mock<IDataRepository>();
            var kvs = new Mock<IKeyValueStore>();

            repo.Setup(r => r.List<TorrentInfo>()).Returns(new List<TorrentInfo>());

            byte[] torrent = TestHelper.LoadResource("Hadouken.UnitTests.Resources.ubuntu.torrent");

            var engine = new MonoTorrentEngine(fs.Object, bus.Object, repo.Object, kvs.Object);
            engine.Load();

            var manager = engine.AddTorrent(torrent);

            Assert.IsNotNull(manager);
            Assert.IsTrue(engine.Managers.ContainsKey(manager.InfoHash));

            engine.RemoveTorrent(manager);

            Assert.IsFalse(engine.Managers.ContainsKey(manager.InfoHash));
        }
    }
}
