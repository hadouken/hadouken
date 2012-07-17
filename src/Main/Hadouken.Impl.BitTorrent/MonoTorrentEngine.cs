﻿using System;
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

namespace Hadouken.Impl.BitTorrent
{
    public class MonoTorrentEngine : IBitTorrentEngine
    {
        private static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        private IDataRepository _data;
        private IFileSystem _fs;
        private IMessageBus _mbus;

        private ClientEngine _clientEngine;
        private Dictionary<string, ITorrentManager> _torrents = new Dictionary<string, ITorrentManager>();

        public MonoTorrentEngine(IFileSystem fs, IMessageBus mbus, IDataRepository data)
        {
            _data = data;
            _fs = fs;
            _mbus = mbus;
        }

        public void Load()
        {
            _logger.Info("Loading BitTorrent engine");

            LoadEngine();
            LoadState();
        }

        private void LoadEngine()
        {
            string defaultSavePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");

            string savePath = _data.GetSetting("bt.savePath", defaultSavePath);
            int listenPort = Convert.ToInt32(_data.GetSetting("bt.listenPort", "6999"));

            _clientEngine = new ClientEngine(new EngineSettings(savePath, listenPort));
        }

        private void LoadState()
        {
            _logger.Info("Loading torrent state");

            foreach (var torrentInfo in _data.List<TorrentInfo>())
            {
                var manager = (HdknTorrentManager)AddTorrent(torrentInfo.Data, torrentInfo.SavePath);

                manager.DownloadedBytes = torrentInfo.DownloadedBytes;
                manager.UploadedBytes = torrentInfo.UploadedBytes;
                manager.Label = torrentInfo.Label;

                if (manager != null)
                {
                    _logger.Debug("Loading FastResume data for torrent {0}", manager.Torrent.Name);

                    manager.LoadFastResume(torrentInfo.FastResumeData);

                    BringToState(manager, torrentInfo.State);
                }
            }
        }

        private void SaveState()
        {
            var infoList = _data.List<TorrentInfo>();

            foreach (TorrentInfo info in infoList)
            {
                if (_torrents.ContainsKey(info.InfoHash))
                {
                    HdknTorrentManager manager = (HdknTorrentManager)_torrents[info.InfoHash];

                    _logger.Debug("Saving state for torrent {0}", manager.Torrent.Name);

                    CreateTorrentInfo(manager, info);
                    _data.Update(info);
                }
                else
                {
                    _data.Delete(info);
                }
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
            if (!manager.HashChecked)
                manager.HashCheck(false);

            info.Data = manager.TorrentData;
            info.DownloadedBytes = manager.DownloadedBytes;
            info.FastResumeData = manager.SaveFastResume();
            info.InfoHash = manager.InfoHash;
            info.Label = manager.Label;
            info.SavePath = manager.SavePath;

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

        public ITorrentManager AddTorrent(byte[] data)
        {
            return AddTorrent(data, String.Empty);
        }

        public ITorrentManager AddTorrent(byte[] data, string savePath)
        {
            if (data == null || data.Length == 0)
                return null;

            Torrent t = null;

            if (Torrent.TryLoad(data, out t))
            {
                TorrentManager manager = new TorrentManager(t, savePath, new TorrentSettings());

                // register with engine
                _clientEngine.Register(manager);

                // add to dictionary
                HdknTorrentManager hdknManager = new HdknTorrentManager(manager, _fs, _mbus);
                hdknManager.Load();

                _torrents.Add(hdknManager.InfoHash, hdknManager);

                _mbus.Send<ITorrentAdded>(m =>
                {
                    m.TorrentManager = hdknManager;
                });

                return hdknManager;
            }

            return null;
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
