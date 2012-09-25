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
using System.IO;

using HdknTorrentState = Hadouken.BitTorrent.TorrentState;
using Hadouken.IO;
using Hadouken.Messaging;
using System.Collections.ObjectModel;
using Hadouken.Messages;
using System.Threading;
using Hadouken.Configuration;
using MonoTorrent;

namespace Hadouken.Impl.BitTorrent
{
    public class MonoTorrentEngine : IBitTorrentEngine
    {
        private static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        private IKeyValueStore _kvs;
        private IDataRepository _data;
        private IFileSystem _fs;
        private IMessageBus _mbus;

        private string _torrentFileSavePath;

        private ClientEngine _clientEngine;
        private Dictionary<string, ITorrentManager> _torrents = new Dictionary<string, ITorrentManager>();

        public MonoTorrentEngine(IFileSystem fs, IMessageBus mbus, IDataRepository data, IKeyValueStore kvs)
        {
            _kvs = kvs;
            _data = data;
            _fs = fs;
            _mbus = mbus;

            _torrentFileSavePath = Path.Combine(HdknConfig.ConfigManager["Paths.Data"], "Torrents");
        }

        public void Load()
        {
            _logger.Info("Loading BitTorrent engine");

            if(!_fs.DirectoryExists(_torrentFileSavePath))
                _fs.CreateDirectory(_torrentFileSavePath);

            LoadEngine();
            LoadState();
        }

        private void LoadEngine()
        {
            string defaultSavePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");

            string savePath = _kvs.Get<string>("bt.savePath", defaultSavePath);
            int listenPort = _kvs.Get<int>("bt.listenPort", 6998);

            _clientEngine = new ClientEngine(new EngineSettings(savePath, listenPort));
        }

        private void LoadState()
        {
            _logger.Info("Loading torrent state");

            var infos = _data.List<TorrentInfo>();

            if(infos == null)
                return;

            foreach (var torrentInfo in infos)
            {
                var manager = (HdknTorrentManager)AddTorrent(torrentInfo.Data, torrentInfo.SavePath);

                if (manager != null)
                {
                    manager.DownloadedBytes = torrentInfo.DownloadedBytes;
                    manager.UploadedBytes = torrentInfo.UploadedBytes;
                    manager.Label = torrentInfo.Label;
                    manager.StartTime = torrentInfo.StartTime;
                    manager.CompletedTime = torrentInfo.CompletedTime;

                    _logger.Debug("Loading FastResume data for torrent {0}", manager.Torrent.Name);

                    if (torrentInfo.FastResumeData != null)
                        manager.LoadFastResume(torrentInfo.FastResumeData);

                    BringToState(manager, torrentInfo.State);
                }
            }
        }

        private void SaveState()
        {
            var infoList = _data.List<TorrentInfo>();

            foreach (var m in _torrents.Values)
            {
                HdknTorrentManager manager = (HdknTorrentManager)m;
                TorrentInfo info = infoList.SingleOrDefault(i => i.InfoHash == manager.InfoHash);

                if (info == null)
                    info = new TorrentInfo();

                _logger.Debug("Saving state for torrent {0}", manager.Torrent.Name);

                CreateTorrentInfo(manager, info);

                _data.SaveOrUpdate(info);
            }
        }

        private void BringToState(HdknTorrentManager manager, HdknTorrentState state)
        {
            switch (state)
            {
                case HdknTorrentState.Downloading:
                case HdknTorrentState.Seeding:
                    manager.Start();
                    break;

                case HdknTorrentState.Hashing:
                    manager.HashCheck(false);
                    break;

                case HdknTorrentState.Paused:
                    manager.Start();
                    manager.Pause();
                    break;

                case HdknTorrentState.Stopped:
                    manager.Stop();
                    break;
            }
        }

        public void Unload()
        {
            SaveState();

            _clientEngine.StopAll();
            _clientEngine.Dispose();
        }

        public IDictionary<string, ITorrentManager> Managers
        {
            get { return (IDictionary<string, ITorrentManager>)_torrents; }
        }

        private void CreateTorrentInfo(HdknTorrentManager manager, TorrentInfo info)
        {
            info.Data = manager.TorrentData;
            info.DownloadedBytes = manager.DownloadedBytes;

            if (manager.HashChecked)
            {
                info.FastResumeData = manager.SaveFastResume();
            }
            else
            {
                info.FastResumeData = null;
            }
            
            info.InfoHash = manager.InfoHash;
            info.Label = manager.Label;
            info.SavePath = manager.SavePath;
            info.StartTime = manager.StartTime;
            info.CompletedTime = manager.CompletedTime;

            // to prevent nesting directories
            if (manager.Torrent.Files.Length > 1)
                info.SavePath = Directory.GetParent(manager.SavePath).FullName;

            info.State = manager.State;
            info.UploadedBytes = manager.UploadedBytes;
        }

        public void StartAll()
        {
            foreach (var manager in _torrents.Values)
                manager.Start();
        }

        public void StopAll()
        {
            foreach (var manager in _torrents.Values)
                manager.Stop();
        }

        public ITorrentManager AddMagnetLink(string url)
        {
            return AddMagnetLink(url, _clientEngine.Settings.SavePath);
        }

        public ITorrentManager AddMagnetLink(string url, string savePath)
        {
            var ml = new MagnetLink(url);
            return RegisterTorrentManager(new TorrentManager(ml, savePath, new TorrentSettings(), _torrentFileSavePath));
        }

        public ITorrentManager AddTorrent(byte[] data)
        {
            return AddTorrent(data, _clientEngine.Settings.SavePath);
        }

        public ITorrentManager AddTorrent(byte[] data, string savePath)
        {
            if (data == null || data.Length == 0)
                return null;

            Torrent t = null;

            if (Torrent.TryLoad(data, out t))
            {
                if (String.IsNullOrEmpty(savePath))
                    savePath = _clientEngine.Settings.SavePath;

                return RegisterTorrentManager(new TorrentManager(t, savePath, new TorrentSettings()), data);
            }

            return null;
        }

        private HdknTorrentManager RegisterTorrentManager(TorrentManager manager, byte[] data = null)
        {
            // register with engine
            _clientEngine.Register(manager);

            // add to dictionary
            var hdknManager = new HdknTorrentManager(manager, _fs, _mbus) { TorrentData = data };
            hdknManager.Load();

            _torrents.Add(hdknManager.InfoHash, hdknManager);

            _mbus.Send<ITorrentAdded>(m =>
            {
                m.Torrent = hdknManager;
            });

            return hdknManager;
        }

        public void RemoveTorrent(ITorrentManager manager)
        {
            var hdknManager = manager as HdknTorrentManager;

            if (hdknManager != null)
            {
                hdknManager.Unload();

                _clientEngine.Unregister(hdknManager.Manager);

                _torrents.Remove(hdknManager.InfoHash);
            }
        }
    }
}
