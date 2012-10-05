using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Hadouken.BitTorrent;
using Hadouken.Configuration;
using MonoTorrent.Client;
using System.IO;
using MonoTorrent.BEncoding;
using Hadouken.IO;
using Hadouken.Messages;
using Hadouken.Messaging;
using System.Threading;

namespace Hadouken.Impl.BitTorrent
{
    public class HdknTorrentManager : ITorrentManager
    {
        private readonly object _lock = new object();
        private readonly object _peersLock = new object();

        private readonly TorrentManager _manager;

        private HdknTorrent _torrent;
        private List<IPeer> _peers = new List<IPeer>();
        private List<ITracker> _trackers = new List<ITracker>();

        private IKeyValueStore _kvs;
        private IFileSystem _fileSystem;
        private IMessageBus _mbus;

        private long _dlBytes;
        private long _ulBytes;
        private DateTime _startTime;
        private DateTime? _completedTime;

        private double _progress;

        internal HdknTorrentManager(TorrentManager manager, IKeyValueStore kvs, IFileSystem fileSystem, IMessageBus mbus)
        {
            _manager = manager;
            _torrent = new HdknTorrent(manager.Torrent);

            foreach (var t in _manager.TrackerManager.TrackerTiers)
            {
                foreach (var t2 in t.GetTrackers())
                {
                    _trackers.Add(new HdknTracker(t2));
                }
            }

            _kvs = kvs;
            _fileSystem = fileSystem;
            _mbus = mbus;
            _startTime = DateTime.Now;
        }

        internal TorrentManager Manager { get { return _manager; } }

        internal void Load()
        {
            _manager.PeerConnected += new EventHandler<PeerConnectionEventArgs>(PeerConnected);
            _manager.PeerDisconnected += new EventHandler<PeerConnectionEventArgs>(PeerDisconnected);
            _manager.PieceHashed += new EventHandler<PieceHashedEventArgs>(PieceHashed);
            _manager.TorrentStateChanged += TorrentStateChanged;
        }

        internal void Unload()
        {
            _manager.PeerConnected -= PeerConnected;
            _manager.PeerDisconnected -= PeerDisconnected;
            _manager.PieceHashed -= PieceHashed;
            _manager.TorrentStateChanged -= TorrentStateChanged;
        }

        private void PeerConnected(object sender, PeerConnectionEventArgs e)
        {
            HdknPeer p = new HdknPeer(e.PeerID);

            lock (_peersLock)
            {
                _peers.Add(p);
            }
        }

        private void PeerDisconnected(object sender, PeerConnectionEventArgs e)
        {
            lock (_peersLock)
            {
                var p = (HdknPeer)_peers.FirstOrDefault(i => i.PeerId == e.PeerID.PeerID);

                if (p != null)
                    _peers.Remove(p);
            }
        }

        private void PieceHashed(object sender, PieceHashedEventArgs e)
        {
            int pieceIndex = e.PieceIndex;
            int totalPieces = e.TorrentManager.Torrent.Pieces.Count;

            lock (_lock)
            {
                _progress = (double)pieceIndex / totalPieces * 100.0;
            }
        }

        private void TorrentStateChanged(object sender, TorrentStateChangedEventArgs e)
        {
            if (e.NewState == MonoTorrent.Common.TorrentState.Error)
            {
                _mbus.Send<ITorrentError>(msg => msg.Torrent = this);
            }

            if (e.OldState == MonoTorrent.Common.TorrentState.Downloading && e.NewState == MonoTorrent.Common.TorrentState.Seeding)
            {
                if (_completedTime == null)
                    _completedTime = DateTime.Now;

                var oldSavePath = String.Copy(SavePath);

                _mbus.Send<ITorrentCompleted>(msg => msg.Torrent = this).ContinueWith(_ =>
                                                                                          {
                                                                                              if(!oldSavePath.Equals(SavePath))
                                                                                                  return;

                                                                                              var moveCompleted =
                                                                                                  _kvs.Get<bool>(
                                                                                                      "paths.shouldMoveCompleted");

                                                                                              var newPath =
                                                                                                  _kvs.Get<string>(
                                                                                                      "paths.completedPath");

                                                                                              if(moveCompleted && !String.IsNullOrEmpty(newPath))
                                                                                              {
                                                                                                  var appendLabel =
                                                                                                      _kvs.Get<bool>(
                                                                                                          "paths.appendLabelOnMoveCompleted");

                                                                                                  if (appendLabel && !String.IsNullOrEmpty(Label))
                                                                                                      newPath =
                                                                                                          Path.Combine(
                                                                                                              newPath,
                                                                                                              Label);

                                                                                                  Move(newPath);
                                                                                              }
                                                                                          });
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
            get { return CalculateCompleted(); }
        }

        private bool CalculateCompleted()
        {
            if (_manager.Complete)
                return true;

            double progress = 0;
            int count = 0;
            foreach (var file in Torrent.Files)
            {
                if (file.Priority != Priority.DoNotDownload)
                {
                    progress += file.BitField.PercentComplete; // This is 0 -> 100.
                    count++;
                }
            }
            progress /= count; 

            return (progress == 100.0);
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
            get 
            {
                if (State == TorrentState.Hashing)
                    return _progress;

                return _manager.Progress;
            }
        }

        public string SavePath
        {
            get { return _manager.SavePath; }
        }

        public DateTime StartTime
        {
            get { return _startTime; }
            internal set { _startTime = value; }
        }

        public DateTime? CompletedTime
        {
            get { return _completedTime; }
            internal set { _completedTime = value; }
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
            get { return CalculateETA(); }
        }

        private TimeSpan CalculateETA()
        {
            if (DownloadSpeed == 0 || Complete)
                return TimeSpan.FromSeconds(-1);

            long downloadedBytes = (long)((double)Torrent.Size * (Progress / 100));

            var s = (Torrent.Size - downloadedBytes) / DownloadSpeed;
            var finishDate = DateTime.Now.AddSeconds(s);

            return finishDate - DateTime.Now;
        }

        public string Label { get; set; }

        public long DownloadedBytes
        {
            get { return _manager.Monitor.DataBytesDownloaded + _dlBytes; }
            internal set { _dlBytes = value; }
        }

        public long RemainingBytes
        {
            get { return CalculateRemaning(); }
        }

        private long CalculateRemaning()
        {
            var files = _manager.Torrent.Files;

            var total = files.Where(f => f.Priority != MonoTorrent.Common.Priority.DoNotDownload).Sum(f => f.Length);
            var dled = files.Where(f => f.Priority != MonoTorrent.Common.Priority.DoNotDownload).Sum(f => f.BytesDownloaded);

            return total - dled;
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
            get { return _peers.ToArray(); }
        }

        public ITracker[] Trackers
        {
            get { return _trackers.ToArray(); }
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
            var isRunning = (State == TorrentState.Seeding || State == TorrentState.Downloading ||
                             State == TorrentState.Metadata || State == TorrentState.Paused ||
                             State == TorrentState.Hashing);

            if (isRunning)
                Stop();

            while(State != TorrentState.Stopped)
                Thread.Sleep(100);

            var oldLocation = SavePath;

            if (Torrent.Files.Length == 1)
                oldLocation = Path.Combine(oldLocation, Torrent.Name);

            var isDirectory = File.GetAttributes(oldLocation).HasFlag(FileAttributes.Directory);

            if (isDirectory)
            {
                if (appendName)
                    newLocation = Path.Combine(newLocation, Torrent.Name);

                DuplicateStructure(oldLocation, newLocation);
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
            if (State == TorrentState.Stopped)
                _manager.HashCheck(autoStart);
        }

        public byte[] SaveFastResume()
        {
            if (!HashChecked)
                HashCheck(false);

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
