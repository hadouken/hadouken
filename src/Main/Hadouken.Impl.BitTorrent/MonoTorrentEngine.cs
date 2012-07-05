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
        private IDataRepository _data;
        private ILogger _logger;
        private IFileSystem _fs;
        private IMessageBus _mbus;

        private ClientEngine _clientEngine;
        private Dictionary<string, HdknTorrent> _torrents = new Dictionary<string, HdknTorrent>();

        public MonoTorrentEngine(ILogger logger, IFileSystem fs, IMessageBus mbus, IDataRepository data)
        {
            _data = data;
            _fs = fs;
            _mbus = mbus;
            _logger = logger;
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
                var torrent = LoadTorrent(torrentInfo.Data, torrentInfo.DownloadedBytes, torrentInfo.UploadedBytes, torrentInfo.SavePath, torrentInfo.Label);

                if (torrent != null)
                {
                    _logger.Debug("Loading FastResume data for torrent {0}", torrent.Name);

                    torrent.Manager.LoadFastResume(new FastResume(BEncodedValue.Decode<BEncodedDictionary>(torrentInfo.FastResumeData)));

                    if (RegisterTorrent(torrent))
                    {
                        BringToState(torrent, torrentInfo.State);
                    }
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
                    HdknTorrent torrent = _torrents[info.InfoHash];

                    _logger.Debug("Saving state for torrent {0}", torrent.Name);

                    CreateTorrentInfo(torrent, info);
                    _data.Update(info);
                }
                else
                {
                    _data.Delete(info);
                }
            }
        }

        private HdknTorrent LoadTorrent(byte[] fileData, long downloadedBytes, long uploadedBytes, string savePath, string label)
        {
            Torrent t = null;

            if (Torrent.TryLoad(fileData, out t))
            {
                if (String.IsNullOrEmpty(savePath))
                    savePath = _clientEngine.Settings.SavePath;

                var torrent = new HdknTorrent(new TorrentManager(t, savePath, new TorrentSettings()), fileData, downloadedBytes, uploadedBytes, label);

                return torrent;
            }

            return null;
        }

        private bool RegisterTorrent(HdknTorrent torrent)
        {
            if (_torrents.ContainsKey(torrent.InfoHash))
                return false;

            // hook up state changed event
            torrent.Manager.TorrentStateChanged += OnTorrentStateChanged;

            // add to dictionary
            _torrents.Add(torrent.InfoHash, torrent);

            // register with engine
            _clientEngine.Register(torrent.Manager);

            return true;
        }

        private void BringToState(HdknTorrent torrent, HdknTorrentState state)
        {
            switch (state)
            {
                case HdknTorrentState.Downloading:
                case HdknTorrentState.Seeding:
                    StartTorrent(torrent);
                    break;

                case HdknTorrentState.Hashing:
                    torrent.Manager.HashCheck(false);
                    break;

                case HdknTorrentState.Paused:
                    StartTorrent(torrent);
                    PauseTorrent(torrent);
                    break;

                case HdknTorrentState.Stopped:
                    StopTorrent(torrent);
                    break;
            }
        }

        private void UnregisterTorrent(HdknTorrent torrent)
        {
            torrent.Manager.TorrentStateChanged -= OnTorrentStateChanged;
            _torrents.Remove(torrent.InfoHash);
            _clientEngine.Unregister(torrent.Manager);
        }

        public void Unload()
        {
            SaveState();

            _clientEngine.StopAll();
            _clientEngine.Dispose();
        }

        public IList<ITorrent> Torrents
        {
            get { return new ReadOnlyCollection<ITorrent>(_torrents.Values.ToList<ITorrent>()); }
        }

        public ITorrent AddTorrent(byte[] data, bool autoStart = false, string savePath = "", string label = "")
        {
            var torrent = LoadTorrent(data, 0, 0, savePath, label);

            if (torrent != null)
            {
                if (RegisterTorrent(torrent))
                {
                    _mbus.Send<ITorrentAdded>(msg => msg.Torrent = torrent);

                    if (autoStart)
                        StartTorrent(torrent);

                    var info = new TorrentInfo();
                    CreateTorrentInfo(torrent, info);
                    _data.Save(info);

                    SaveState();

                    return torrent;
                }
            }

            return null;
        }

        private void CreateTorrentInfo(HdknTorrent torrent, TorrentInfo info)
        {
            byte[] fastResume = null;

            using(var ms = new MemoryStream()) {
                torrent.Manager.SaveFastResume().Encode(ms);
                fastResume = ms.ToArray();
            }

            info.Data = torrent.FileData;
            info.DownloadedBytes = torrent.DownloadedBytes;
            info.FastResumeData = fastResume;
            info.InfoHash = torrent.InfoHash;
            info.Label = torrent.Label;
            info.SavePath = torrent.SavePath;
            info.State = torrent.State;
            info.UploadedBytes = torrent.UploadedBytes;
        }

        public void RemoveTorrent(ITorrent torrent)
        {
            if (torrent == null)
                return;

            if (_torrents.ContainsKey(torrent.InfoHash))
            {
                StopTorrent(torrent);
                UnregisterTorrent(_torrents[torrent.InfoHash]);

                var info = _data.Single<TorrentInfo>(x => x.InfoHash == torrent.InfoHash);
                _data.Delete(info);
            }
        }

        public void RemoveTorrent(string infoHash)
        {
            if (_torrents.ContainsKey(infoHash))
                RemoveTorrent(_torrents[infoHash]);
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
            if (_torrents.ContainsKey(infoHash))
                StartTorrent(_torrents[infoHash]);
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
            if (_torrents.ContainsKey(infoHash))
                StopTorrent(_torrents[infoHash]);
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
            if (_torrents.ContainsKey(infoHash))
                PauseTorrent(_torrents[infoHash]);
        }

        public void MoveTorrent(ITorrent torrent, string newPath, bool appendName = false)
        {
            if (!_torrents.ContainsKey(torrent.InfoHash))
                return;

            var hdknTorrent = _torrents[torrent.InfoHash];

            var isRunning = (hdknTorrent.State == HdknTorrentState.Downloading || hdknTorrent.State == HdknTorrentState.Seeding);
            if (isRunning)
                StopTorrent(hdknTorrent);

            var isDirectory = File.GetAttributes(hdknTorrent.SavePath).HasFlag(FileAttributes.Directory);
            var oldLocation = hdknTorrent.SavePath;

            if (isDirectory)
            {
                if (appendName)
                    newPath = Path.Combine(newPath, torrent.Name);

                DuplicateStructure(hdknTorrent.SavePath, newPath);
            }

            hdknTorrent.Manager.MoveFiles(newPath, true);

            if (isDirectory)
                _fs.DeleteDirectory(oldLocation);

            if (isRunning)
                StartTorrent(hdknTorrent);

            _mbus.Send<ITorrentMoved>(m =>
            {
                m.Torrent = hdknTorrent;
                m.OldPath = oldLocation;
                m.NewPath = hdknTorrent.SavePath;
            });
        }

        private void DuplicateStructure(string source, string target)
        {
            if (!_fs.DirectoryExists(target))
                _fs.CreateDirectory(target);

            foreach (string subDirectory in _fs.GetDirectories(source))
            {
                var name = new DirectoryInfo(subDirectory).Name;
                DuplicateStructure(Path.Combine(source, name), Path.Combine(target, name));
            }
        }

        public void MoveTorrent(string infoHash, string newPath, bool appendName = false)
        {
            if (_torrents.ContainsKey(infoHash))
                MoveTorrent(_torrents[infoHash], newPath);
        }

        private void OnTorrentStateChanged(object sender, TorrentStateChangedEventArgs args)
        {
            //
        }
    }
}
