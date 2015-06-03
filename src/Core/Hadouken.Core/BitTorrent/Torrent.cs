using System;
using System.Collections.Generic;
using System.Linq;
using Hadouken.Common.BitTorrent;
using Ragnar;
using TorrentState = Hadouken.Common.BitTorrent.TorrentState;

namespace Hadouken.Core.BitTorrent {
    internal sealed class Torrent : ITorrent, IDisposable {
        private readonly TorrentHandle _handle;
        private TorrentInfo _info;
        private TorrentStatus _status;

        public Torrent(TorrentHandle handle) {
            if (handle == null) {
                throw new ArgumentNullException("handle");
            }
            this._handle = handle;
            this._status = handle.QueryStatus();

            // Set initial properties
            this.TorrentInfo = this._handle.TorrentFile;
        }

        internal TorrentHandle Handle {
            get { return this._handle; }
        }

        internal TorrentInfo TorrentInfo {
            get { return this._info; }
            set {
                if (this._info != null) {
                    this._info.Dispose();
                }
                this._info = value;
            }
        }

        internal TorrentStatus Status {
            get { return this._status; }
            set {
                if (this._status != null) {
                    this._status.Dispose();
                }
                this._status = value;
            }
        }

        public bool HasMetadata {
            get { return this.TorrentInfo != null; }
        }

        public void Dispose() {
            if (this.TorrentInfo != null) {
                this.TorrentInfo.Dispose();
            }

            this._status.Dispose();
            this._handle.Dispose();
        }

        public string InfoHash {
            get { return this._status.InfoHash.ToHex(); }
        }

        public string Name {
            get { return (this.HasMetadata ? this._status.Name : this.InfoHash); }
        }

        public long Size {
            get { return this.HasMetadata ? this.TorrentInfo.TotalSize : -1; }
        }

        public float Progress {
            get { return this._status.Progress; }
        }

        public string SavePath {
            get { return this._status.SavePath; }
        }

        public long DownloadSpeed {
            get { return this._status.DownloadPayloadRate; }
        }

        public long UploadSpeed {
            get { return this._status.UploadPayloadRate; }
        }

        public long DownloadedBytes {
            get { return this._status.TotalPayloadDownload; }
        }

        public long UploadedBytes {
            get { return this._status.TotalPayloadUpload; }
        }

        public long TotalDownloadedBytes {
            get { return this._status.AllTimeDownload; }
        }

        public long TotalUploadedBytes {
            get { return this._status.AllTimeUpload; }
        }

        public long TotalRemainingBytes {
            get { return this._status.TotalWanted - this._status.TotalWantedDone; }
        }

        public TorrentState State {
            get { return (TorrentState) (int) this._status.State; }
        }

        public bool Paused {
            get { return this._status.Paused; }
        }

        public string Label { get; internal set; }

        public bool IsSeed {
            get { return this._status.IsSeeding; }
        }

        public bool IsFinished {
            get { return this._status.IsFinished; }
        }

        public int QueuePosition {
            get { return this._status.QueuePosition; }
        }

        public string Error {
            get { return this._status.Error; }
        }

        public ITorrentSettings GetSettings() {
            return new TorrentSettings {
                DownloadRateLimit = this._handle.DownloadLimit,
                MaxConnections = this._handle.MaxConnections,
                MaxUploads = this._handle.MaxUploads,
                SequentialDownload = this._handle.SequentialDownload,
                UploadRateLimit = this._handle.UploadLimit
            };
        }

        public IEnumerable<ITorrentFile> GetFiles() {
            if (!this.HasMetadata) {
                return Enumerable.Empty<ITorrentFile>();
            }

            var result = new List<ITorrentFile>();

            for (var i = 0; i < this.TorrentInfo.NumFiles; i++) {
                using (var entry = this.TorrentInfo.FileAt(i)) {
                    result.Add(new TorrentFile(i, entry.Path, entry.Size, entry.Offset));
                }
            }

            return result;
        }

        public IEnumerable<int> GetFilePriorities() {
            return !this.HasMetadata ? Enumerable.Empty<int>() : this._handle.GetFilePriorities();
        }

        public IEnumerable<float> GetFileProgress() {
            if (!this.HasMetadata) {
                return Enumerable.Empty<float>();
            }

            var progress = this._handle.GetFileProgresses();
            var result = new List<float>();

            foreach (var file in this.GetFiles()) {
                try {
                    result.Add(progress[file.Index]/(float) file.Size);
                }
                catch (DivideByZeroException) {
                    result.Add(0.0f);
                }
            }

            return result;
        }

        public IEnumerable<IPeer> GetPeers() {
            var result = new List<IPeer>();
            var peers = this._handle.GetPeerInfo();

            foreach (var peer in peers.Where(peer => !peer.Flags.HasFlag(PeerFlags.Connecting) && !peer.Flags.HasFlag(PeerFlags.Handshake))) {
                result.Add(new Peer {
                    Client = peer.Client,
                    Country = string.Empty,
                    DownloadSpeed = peer.PayloadDownSpeed,
                    Ip = peer.EndPoint.ToString(),
                    IsSeed = peer.Flags.HasFlag(PeerFlags.Seed),
                    Progress = peer.Progress,
                    UploadSpeed = peer.PayloadUpSpeed
                });

                peer.Dispose();
            }

            return result;
        }
    }
}