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
using System.Net;
using EncryptionTypes = MonoTorrent.Client.Encryption.EncryptionTypes;

namespace Hadouken.Impl.BitTorrent
{
    [Component]
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

            _mbus.Subscribe<ISettingChanged>(SettingChanged);

            _torrentFileSavePath = Path.Combine(HdknConfig.GetPath("Paths.Data"), "Torrents");
        }

        public void Load()
        {
            _logger.Info("Loading BitTorrent engine");

            if(!_fs.DirectoryExists(_torrentFileSavePath))
                _fs.CreateDirectory(_torrentFileSavePath);

            LoadEngine();
            LoadState();
        }

        private void SettingChanged(ISettingChanged message)
        {
            if(_clientEngine == null)
                return;

            switch(message.Key)
            {
                case "bandwidth.globalMaxConnections":
                    _clientEngine.Settings.GlobalMaxConnections = Convert.ToInt32(message.NewValue);
                    break;

                case "bt.listenPort":
                    var newPort = Convert.ToInt32(message.NewValue);
                    _clientEngine.ChangeListenEndpoint(new IPEndPoint(IPAddress.Any, newPort));
                    break;

                case "paths.defaultSavePath":
                    _clientEngine.Settings.SavePath = message.NewValue.ToString();
                    break;
            }
        }

        private void LoadEngine()
        {
            string defaultSavePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
            string savePath = _kvs.Get<string>("paths.defaultSavePath", defaultSavePath);
            int listenPort = _kvs.Get<int>("bt.listenPort", 6998);

            var settings = new EngineSettings
                               {
                                   AllowedEncryption = EncryptionTypes.All,
                                   GlobalMaxConnections = _kvs.Get("bandwidth.globalMaxConnections", 200),
                                   GlobalMaxDownloadSpeed = _kvs.Get("bandwidth.globalMaxDlSpeed", 0),
                                   GlobalMaxHalfOpenConnections = _kvs.Get("bandwidth.globalMaxHalfConnections", 100),
                                   GlobalMaxUploadSpeed = _kvs.Get("bandwidth.globalMaxUpSpeed", 0),
                                   //HaveSupressionEnabled
                                   MaxOpenFiles = _kvs.Get("diskio.maxOpenFiles", 0),
                                   MaxReadRate = _kvs.Get("diskio.maxReadRate", 0),
                                   MaxWriteRate = _kvs.Get("diskio.maxWriteRate", 0),
                                   PreferEncryption = _kvs.Get("bt.preferEncryption", true),
                                   //ReportedAddress = _kvs.Get<string>("bt.reportedAddress")
                                   SavePath = savePath
                               };

            _clientEngine = new ClientEngine(settings);
            _clientEngine.ChangeListenEndpoint(new IPEndPoint(IPAddress.Any, listenPort));
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

                    // Load settings
                    manager.Settings.ConnectionRetentionFactor = torrentInfo.ConnectionRetentionFactor;
                    manager.Settings.EnablePeerExchange = torrentInfo.EnablePeerExchange;
                    manager.Settings.InitialSeedingEnabled = torrentInfo.InitialSeedingEnabled;
                    manager.Settings.MaxConnections = torrentInfo.MaxConnections;
                    manager.Settings.MaxDownloadSpeed = torrentInfo.MaxDownloadSpeed;
                    manager.Settings.MaxUploadSpeed = torrentInfo.MaxUploadSpeed;
                    manager.Settings.MinimumTimeBetweenReviews = torrentInfo.MinimumTimeBetweenReviews;
                    manager.Settings.PercentOfMaxRateToSkipReview = torrentInfo.PercentOfMaxRateToSkipReview;
                    manager.Settings.UploadSlots = torrentInfo.UploadSlots;
                    manager.Settings.UseDht = torrentInfo.UseDht;

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

            // save torrent settings
            info.ConnectionRetentionFactor = manager.Settings.ConnectionRetentionFactor;
            info.EnablePeerExchange = manager.Settings.EnablePeerExchange;
            info.InitialSeedingEnabled = manager.Settings.InitialSeedingEnabled;
            info.MaxConnections = manager.Settings.MaxConnections;
            info.MaxDownloadSpeed = manager.Settings.MaxDownloadSpeed;
            info.MaxUploadSpeed = manager.Settings.MaxUploadSpeed;
            info.MinimumTimeBetweenReviews = manager.Settings.MinimumTimeBetweenReviews;
            info.PercentOfMaxRateToSkipReview = manager.Settings.PercentOfMaxRateToSkipReview;

            info.UploadSlots = manager.Settings.UploadSlots;
            info.UseDht = manager.Settings.UseDht;
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

                return RegisterTorrentManager(new TorrentManager(t, savePath, new TorrentSettings()), data); ;
            }

            return null;
        }

        private HdknTorrentManager RegisterTorrentManager(TorrentManager manager, byte[] data = null)
        {
            // register with engine
            if (_clientEngine.Contains(manager))
                return null;

            _clientEngine.Register(manager);

            // add to dictionary
            var hdknManager = new HdknTorrentManager(manager, _kvs, _fs, _mbus) { TorrentData = data };
            hdknManager.Load();

            _torrents.Add(hdknManager.InfoHash, hdknManager);

            _mbus.Send<ITorrentAdded>(m =>
            {
                m.Torrent = hdknManager;
            });

            // Save state whenever adding torrents.
            SaveState();

            return hdknManager;
        }

        public void RemoveTorrent(ITorrentManager manager)
        {
            var hdknManager = manager as HdknTorrentManager;

            if (hdknManager != null)
            {
                // Stop torrent

                hdknManager.Stop();

                while(hdknManager.State != HdknTorrentState.Stopped)
                    Thread.Sleep(100);

                hdknManager.Unload();

                _clientEngine.Unregister(hdknManager.Manager);

                _torrents.Remove(hdknManager.InfoHash);
            }
        }
    }
}
