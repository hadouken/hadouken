using System;
using System.Collections.Generic;
using System.Linq;
using Hadouken.Common.BitTorrent;
using Ragnar;

namespace Hadouken.Core.BitTorrent
{
    internal sealed class Torrent : ITorrent, IDisposable
    {
        private readonly TorrentHandle _handle;
        private TorrentInfo _info;
        private TorrentStatus _status;

        public Torrent(TorrentHandle handle)
        {
            if (handle == null) throw new ArgumentNullException("handle");
            _handle = handle;
            _status = handle.QueryStatus();

            // Set initial properties
            TorrentInfo = _handle.TorrentFile;
        }

        internal TorrentHandle Handle
        {
            get { return _handle; }
        }

        internal TorrentInfo TorrentInfo
        {
            get { return _info; }
            set
            {
                if (_info != null) _info.Dispose();
                _info = value;
            }
        }

        internal TorrentStatus Status
        {
            get { return _status; }
            set
            {
                if (_status != null) _status.Dispose();
                _status = value;
            }
        }

        public string InfoHash
        {
            get { return _handle.InfoHash.ToHex(); }
        }

        public string Name
        {
            get { return GetName(); }
        }

        public bool HasMetadata
        {
            get { return TorrentInfo != null; }
        }

        public long Size
        {
            get { return HasMetadata ? TorrentInfo.TotalSize : -1; }
        }

        public float Progress
        {
            get { return _status.Progress; }
        }

        public string SavePath
        {
            get { return GetSavePath(); }
        }

        public long DownloadSpeed
        {
            get { return _status.DownloadPayloadRate; }
        }

        public long UploadSpeed
        {
            get { return _status.UploadPayloadRate; }
        }

        public long TotalDownloadedBytes
        {
            get { return _status.TotalPayloadDownload; }
        }

        public long TotalUploadedBytes
        {
            get { return _status.TotalPayloadUpload; }
        }

        public Common.BitTorrent.TorrentState State
        {
            get { return (Common.BitTorrent.TorrentState) (int) _status.State; }
        }

        public bool Paused
        {
            get { return _status.Paused; }
        }

        public string Label { get; private set; }

        public bool IsSeed
        {
            get { return _status.IsSeeding; }
        }

        public bool IsFinished
        {
            get { return _status.IsFinished; }
        }

        public int QueuePosition
        {
            get { return _handle.QueuePosition; }
        }

        public IEnumerable<ITorrentFile> GetFiles()
        {
            if (!HasMetadata)
            {
                return Enumerable.Empty<ITorrentFile>();
            }

            var result = new List<ITorrentFile>();

            for (var i = 0; i < TorrentInfo.NumFiles; i++)
            {
                using (var entry = TorrentInfo.FileAt(i))
                {
                    result.Add(new TorrentFile(i, entry.Path, entry.Size, entry.Offset));
                }
            }

            return result;
        }

        public IEnumerable<int> GetFilePriorities()
        {
            return !HasMetadata ? Enumerable.Empty<int>() : _handle.GetFilePriorities();
        }

        public IEnumerable<float> GetFileProgress()
        {
            if (!HasMetadata)
            {
                return Enumerable.Empty<float>();
            }

            var progress = _handle.GetFileProgresses();
            var result = new List<float>();

            foreach (var file in GetFiles())
            {
                try
                {
                    result.Add(progress[file.Index]/(float) file.Size);
                }
                catch (DivideByZeroException)
                {
                    result.Add(0.0f);
                }
            }

            return result;
        } 

        public IEnumerable<IPeer> GetPeers()
        {
            var result = new List<IPeer>();
            var peers = _handle.GetPeerInfo();

            foreach (var peer in peers)
            {
                if (peer.Flags.HasFlag(PeerFlags.Connecting)
                    || peer.Flags.HasFlag(PeerFlags.Handshake))
                {
                    continue;
                }

                result.Add(new Peer
                {
                    Client = peer.Client,
                    Country = string.Empty,
                    DownloadSpeed = peer.PayloadDownSpeed,
                    IP = peer.EndPoint.ToString(),
                    IsSeed = peer.Flags.HasFlag(PeerFlags.Seed),
                    Progress = peer.Progress,
                    UploadSpeed = peer.PayloadUpSpeed
                });

                peer.Dispose();
            }

            return result;
        }

        private string GetName()
        {
            if (!HasMetadata) return InfoHash;

            using (var entry = TorrentInfo.FileAt(0))
            {
                var name = entry.Path.Replace("\\", "/").Split('/').FirstOrDefault();
                return string.IsNullOrEmpty(name) ? TorrentInfo.Name : name;
            }
        }

        private string GetSavePath()
        {
            return (HasMetadata && TorrentInfo.NumFiles > 1
                ? System.IO.Path.Combine(_status.SavePath, GetName())
                : _status.SavePath);
        }

        public void Dispose()
        {
            if (TorrentInfo != null)
            {
                TorrentInfo.Dispose();
            }

            _status.Dispose();
            _handle.Dispose();
        }
    }
}
