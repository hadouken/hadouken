using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Hadouken.BitTorrent;
using Hadouken.Data;
using MonoTorrent.Client;
using Hadouken.Data.Models;
using MonoTorrent.Common;
using MonoTorrent.BEncoding;
using Hadouken.Logging;

namespace Hadouken.Impl.BitTorrent
{
    public class MonoTorrentEngine : IBitTorrentEngine
    {
        private IDataRepository _data;
        private ILogger _logger;

        private ClientEngine _clientEngine;
        private List<HdknTorrent> _torrents = new List<HdknTorrent>();

        public MonoTorrentEngine(ILogger logger, IDataRepository data)
        {
            _data = data;
            _logger = logger;
        }

        public void Load()
        {
            _logger.Info("Loading BitTorrent engine");

            string defaultSavePath = _data.GetSetting("bt.savePath", @"C:\Downloads");
            int listenPort = Convert.ToInt32(_data.GetSetting("bt.listenPort", "6999"));

            _clientEngine = new ClientEngine(new EngineSettings(defaultSavePath, listenPort));

            LoadState();
        }

        private void LoadState()
        {
            foreach (var torrentInfo in _data.List<TorrentInfo>())
            {
                Torrent t = null;

                if (Torrent.TryLoad(torrentInfo.Data, out t))
                {
                    var manager = new TorrentManager(t, torrentInfo.SavePath, new TorrentSettings());

                    BEncodedDictionary bencodedResume = BEncodedValue.Decode<BEncodedDictionary>(torrentInfo.FastResumeData);
                    FastResume resume = new FastResume(bencodedResume);

                    manager.LoadFastResume(resume);

                    _clientEngine.Register(manager);
                }
            }
        }

        public void Unload()
        {
            throw new NotImplementedException();
        }

        public IList<ITorrent> Torrents
        {
            get { return _torrents.ToList<ITorrent>(); }
        }

        public ITorrent AddTorrent(byte[] data, bool autoStart = false, string savePath = "", string label = "")
        {
            Torrent t = null;

            if (Torrent.TryLoad(data, out t))
            {
                //
            }

            return null;
        }

        public void RemoveTorrent(ITorrent torrent)
        {
            if (torrent == null)
                return;

            var hdknTorrent = (HdknTorrent)torrent;
            hdknTorrent.Manager.Stop();

            _clientEngine.Unregister(hdknTorrent.Manager);
            _torrents.Remove(hdknTorrent);
        }

        public void RemoveTorrent(string infoHash)
        {
            RemoveTorrent(_torrents.SingleOrDefault(x => x.InfoHash == infoHash));
        }

        public void StartTorrent(ITorrent torrent)
        {
            if (torrent == null)
                return;

            var hdknTorrent = (HdknTorrent)torrent;
            hdknTorrent.Manager.Start();
        }

        public void StartTorrent(string infoHash)
        {
            StartTorrent(_torrents.SingleOrDefault(x => x.InfoHash == infoHash));
        }

        public void StopTorrent(ITorrent torrent)
        {
            if (torrent == null)
                return;

            var hdknTorrent = (HdknTorrent)torrent;
            hdknTorrent.Manager.Stop();
        }

        public void StopTorrent(string infoHash)
        {
            StopTorrent(_torrents.SingleOrDefault(x => x.InfoHash == infoHash));
        }

        public void PauseTorrent(ITorrent torrent)
        {
            if (torrent == null)
                return;

            var hdknTorrent = (HdknTorrent)torrent;
            hdknTorrent.Manager.Pause();
        }

        public void PauseTorrent(string infoHash)
        {
            PauseTorrent(_torrents.SingleOrDefault(x => x.InfoHash == infoHash));
        }

        public void MoveTorrent(ITorrent torrent, string newPath)
        {
            throw new NotImplementedException();
        }

        public void MoveTorrent(string infoHash, string newPath)
        {
            MoveTorrent(_torrents.SingleOrDefault(x => x.InfoHash == infoHash), newPath);
        }
    }
}
