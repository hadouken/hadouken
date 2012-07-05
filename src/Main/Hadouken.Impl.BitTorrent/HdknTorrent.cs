using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Hadouken.BitTorrent;
using MonoTorrent.Client;

namespace Hadouken.Impl.BitTorrent
{
    public class HdknTorrent : ITorrent
    {
        private TorrentManager _manager;
        private string _label;
        private long _dlBytes;
        private long _ulBytes;

        internal HdknTorrent(TorrentManager tm, byte[] fileData, long dlBytes, long ulBytes, string label)
        {
            _manager = tm;
            _dlBytes = dlBytes;
            _ulBytes = ulBytes;
            _label = label;

            FileData = fileData;
        }

        internal TorrentManager Manager
        {
            get { return _manager; }
        }

        internal byte[] FileData { get; private set; }

        public string Name
        {
            get { return _manager.Torrent.Name; }
        }

        public string InfoHash
        {
            get { return _manager.InfoHash.ToString().Replace("-", ""); }
        }

        public long Size
        {
            get { return _manager.Torrent.Size; }
        }

        public long DownloadedBytes
        {
            get { return _dlBytes + _manager.Monitor.DataBytesDownloaded; }
        }

        public long UploadedBytes
        {
            get { return _ulBytes + _manager.Monitor.DataBytesUploaded; }
        }

        public long DownloadSpeed
        {
            get { return _manager.Monitor.DownloadSpeed; }
        }

        public long UploadSpeed
        {
            get { return _manager.Monitor.UploadSpeed; }
        }

        public double Progress
        {
            get { return _manager.Progress; }
        }

        public string SavePath
        {
            get { return _manager.SavePath; }
        }

        public string Label
        {
            get { return _label; }
            set { _label = value; }
        }

        public TorrentState State
        {
            get { return (TorrentState)(int)_manager.State; }
        }

        public bool IsMultiFile
        {
            get { return _manager.Torrent.Files.Length > 1; }
        }

        public IList<ITorrentFile> Files
        {
            get { throw new NotImplementedException(); }
        }

        public IList<IPeer> Peers
        {
            get { throw new NotImplementedException(); }
        }

        public IList<ITracker> Trackers
        {
            get { throw new NotImplementedException(); }
        }
    }
}
