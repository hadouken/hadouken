using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Hadouken.BitTorrent;
using MonoTorrent.Client;
using System.IO;
using MonoTorrent.BEncoding;
using Hadouken.IO;
using Hadouken.Messages;
using Hadouken.Messaging;

namespace Hadouken.Impl.BitTorrent
{
    public class HdknTorrentManager : ITorrentManager
    {
        private TorrentManager _manager;

        private HdknTorrent _torrent;
        private List<HdknPeer> _peers = new List<HdknPeer>();

        private IFileSystem _fileSystem;
        private IMessageBus _mbus;

        private long _dlBytes;
        private long _ulBytes;

        internal HdknTorrentManager(TorrentManager manager, IFileSystem fileSystem, IMessageBus mbus)
        {
            _manager = manager;
            _torrent = new HdknTorrent(manager.Torrent);

            _fileSystem = fileSystem;
            _mbus = mbus;
        }

        internal TorrentManager Manager { get { return _manager; } }

        internal void Load()
        {
            _manager.TorrentStateChanged += TorrentStateChanged;
        }

        internal void Unload()
        {
            _manager.TorrentStateChanged -= TorrentStateChanged;
        }

        private void TorrentStateChanged(object sender, TorrentStateChangedEventArgs e)
        {
            if (e.NewState == MonoTorrent.Common.TorrentState.Error)
            {
                _mbus.Send<ITorrentError>(msg => msg.Torrent = this);
            }

            if (e.OldState == MonoTorrent.Common.TorrentState.Downloading && e.NewState == MonoTorrent.Common.TorrentState.Seeding)
            {
                _mbus.Send<ITorrentCompleted>(msg => msg.Torrent = this);
            }
        }

        public IBitField BitField
        {
            get { return new HdknBitField(_manager.Bitfield); }
        }

        public bool CanUseDht
        {
            get { return _manager.CanUseDht; }
        }

        public bool Complete
        {
            get { return _manager.Complete; }
        }

        public bool HashChecked
        {
            get { return _manager.HashChecked; }
        }

        public int HashFails
        {
            get { return _manager.HashFails; }
        }

        public bool HasMetadata
        {
            get { return _manager.HasMetadata; }
        }

        public List<Uri> InactivePeerList
        {
            get { return _manager.InactivePeerList; }
        }

        public int InactivePeers
        {
            get { return _manager.InactivePeers; }
        }

        public string InfoHash
        {
            get { return _manager.InfoHash.ToString().Replace("-", ""); }
        }

        public bool IsInEndGame
        {
            get { return _manager.IsInEndGame; }
        }

        public bool IsInitialSeeding
        {
            get { return _manager.IsInitialSeeding; }
        }

        public int OpenConnections
        {
            get { return _manager.OpenConnections; }
        }

        public int PeerReviewRoundsComplete
        {
            get { return _manager.PeerReviewRoundsComplete; }
        }

        public double Progress
        {
            get { return _manager.Progress; }
        }

        public string SavePath
        {
            get { return _manager.SavePath; }
        }

        public DateTime StartTime
        {
            get { return _manager.StartTime; }
        }

        public DateTime? CompletedTime
        {
            get { return null; }
        }

        public TorrentState State
        {
            get { return (TorrentState)(int)_manager.State; }
        }

        public int UploadingTo
        {
            get { return _manager.UploadingTo; }
        }

        public TimeSpan ETA
        {
            get { throw new NotImplementedException(); }
        }

        public string Label { get; set; }

        public long DownloadedBytes
        {
            get { return _manager.Monitor.DataBytesDownloaded + _dlBytes; }
            internal set { _dlBytes = value; }
        }

        public long UploadedBytes
        {
            get { return _manager.Monitor.DataBytesUploaded + _ulBytes; }
            internal set { _ulBytes = value; }
        }

        public long DownloadSpeed
        {
            get { return _manager.Monitor.DownloadSpeed; }
        }

        public long UploadSpeed
        {
            get { return _manager.Monitor.UploadSpeed; }
        }

        public ITorrent Torrent
        {
            get { return _torrent; }
        }

        public byte[] TorrentData { get; internal set; }

        public IPeer[] Peers
        {
            get { return _peers.ToArray<IPeer>(); }
        }

        public void Start()
        {
            _manager.Start();
        }

        public void Stop()
        {
            _manager.Stop();
        }

        public void Pause()
        {
            _manager.Pause();
        }

        public void Move(string newLocation, bool appendName = false)
        {
            var isRunning = (State == TorrentState.Seeding || State == TorrentState.Downloading);

            if (isRunning)
                Stop();

            var isDirectory = File.GetAttributes(SavePath).HasFlag(FileAttributes.Directory);
            var oldLocation = SavePath;

            if (isDirectory)
            {
                if (appendName)
                    newLocation = Path.Combine(newLocation, Torrent.Name);

                DuplicateStructure(SavePath, newLocation);
            }

            _manager.MoveFiles(newLocation, true);

            if (isDirectory)
                _fileSystem.DeleteDirectory(oldLocation);

            if (isRunning)
                Start();

            _mbus.Send<ITorrentMoved>(m =>
            {
                m.TorrentManager = this;
                m.OldPath = oldLocation;
                m.NewPath = SavePath;
            });
        }

        private void DuplicateStructure(string source, string target)
        {
            if (!_fileSystem.DirectoryExists(target))
                _fileSystem.CreateDirectory(target);

            foreach (string subDirectory in _fileSystem.GetDirectories(source))
            {
                var name = new DirectoryInfo(subDirectory).Name;
                DuplicateStructure(Path.Combine(source, name), Path.Combine(target, name));
            }
        }

        public void HashCheck(bool autoStart)
        {
            _manager.HashCheck(autoStart);
        }

        public byte[] SaveFastResume()
        {
            using (var ms = new MemoryStream())
            {
                _manager.SaveFastResume().Encode(ms);
                return ms.ToArray();
            }
        }

        public void LoadFastResume(byte[] data)
        {
            var d = BEncodedValue.Decode<BEncodedDictionary>(data);
            var f = new FastResume(d);

            _manager.LoadFastResume(f);
        }
    }
}
