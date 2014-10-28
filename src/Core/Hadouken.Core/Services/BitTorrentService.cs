using System;
using System.Collections.Generic;
using System.Linq;
using Hadouken.Common.BitTorrent;
using Hadouken.Common.JsonRpc;
using Hadouken.Common.Messaging;
using Hadouken.Core.Services.Models;

namespace Hadouken.Core.Services
{
    public sealed class BitTorrentService : IJsonRpcService
    {
        private readonly IMessageBus _messageBus;
        private readonly ITorrentEngine _torrentEngine;

        public BitTorrentService(IMessageBus messageBus, ITorrentEngine torrentEngine)
        {
            if (messageBus == null) throw new ArgumentNullException("messageBus");
            if (torrentEngine == null) throw new ArgumentNullException("torrentEngine");
            _messageBus = messageBus;
            _torrentEngine = torrentEngine;
        }

        [JsonRpcMethod("torrents.getAll")]
        public IEnumerable<ITorrent> GetAll()
        {
            return _torrentEngine.GetAll();
        }

        [JsonRpcMethod("torrents.getByInfoHashList")]
        public IEnumerable<ITorrent> GetByInfoHashList(string[] infoHashList)
        {
            return infoHashList.Select(infoHash => _torrentEngine.GetByInfoHash(infoHash));
        }

        [JsonRpcMethod("torrents.getByInfoHash")]
        public ITorrent GetByInfoHash(string infoHash)
        {
            return _torrentEngine.GetByInfoHash(infoHash);
        }

        [JsonRpcMethod("torrents.getLabels")]
        public IEnumerable<string> GetLabels()
        {
            return _torrentEngine.GetLabels();
        }

        [JsonRpcMethod("torrents.getFiles")]
        public IEnumerable<ITorrentFile> GetFiles(string infoHash)
        {
            var torrent = _torrentEngine.GetByInfoHash(infoHash);
            return torrent == null ? null : torrent.GetFiles();
        }

        [JsonRpcMethod("torrents.getFileProgress")]
        public IEnumerable<float> GetFileProgress(string infoHash)
        {
            var torrent = _torrentEngine.GetByInfoHash(infoHash);
            return torrent == null ? null : torrent.GetFileProgress();
        }

        [JsonRpcMethod("torrents.getFilePriorities")]
        public IEnumerable<int> GetFilePriorities(string infoHash)
        {
            var torrent = _torrentEngine.GetByInfoHash(infoHash);
            return torrent == null ? null : torrent.GetFilePriorities();
        }

        [JsonRpcMethod("torrents.getPeers")]
        public IEnumerable<IPeer> GetPeers(string infoHash)
        {
            var torrent = _torrentEngine.GetByInfoHash(infoHash);
            return torrent == null ? null : torrent.GetPeers();
        }

        [JsonRpcMethod("torrents.getSettings")]
        public ITorrentSettings GetSettings(string infoHash)
        {
            var torrent = _torrentEngine.GetByInfoHash(infoHash);
            return torrent == null ? null : torrent.GetSettings();
        }

        [JsonRpcMethod("torrents.setSettings")]
        public void SetSettings(string infoHash, TorrentSettings settings)
        {
            if (settings == null) return;
            _messageBus.Publish(new ChangeTorrentSettingsMessage(infoHash,
                settings.DownloadRateLimit,
                settings.MaxConnections,
                settings.MaxUploads,
                settings.SequentialDownload,
                settings.UploadRateLimit));
        }
        
        [JsonRpcMethod("torrents.addFile")]
        public void AddFile(byte[] data, TorrentParameters parameters)
        {
            var msg = new AddTorrentMessage(data)
            {
                Label = parameters.Label,
                SavePath = parameters.SavePath
            };

            _messageBus.Publish(msg);
        }

        [JsonRpcMethod("torrents.addUrl")]
        public void AddMagnetLink(string url, TorrentParameters parameters)
        {
            var msg = new AddUrlMessage(url)
            {
                Label = parameters.Label,
                Name = parameters.Name,
                SavePath = parameters.SavePath,
                Trackers = parameters.Trackers
            };

            _messageBus.Publish(msg);
        }

        [JsonRpcMethod("torrents.pause")]
        public void Pause(string infoHash)
        {
            _messageBus.Publish(new PauseTorrentMessage(infoHash));
        }

        [JsonRpcMethod("torrents.resume")]
        public void Resume(string infoHash)
        {
            _messageBus.Publish(new ResumeTorrentMessage(infoHash));
        }

        [JsonRpcMethod("torrents.remove")]
        public void Remove(string infoHash, bool removeData)
        {
            _messageBus.Publish(new RemoveTorrentMessage(infoHash) {RemoveData = removeData});
        }

        [JsonRpcMethod("torrents.move")]
        public void Move(string infoHash, string destination, bool overwriteExisting)
        {
            _messageBus.Publish(new MoveTorrentMessage(infoHash, destination) {OverwriteExisting = overwriteExisting});
        }

        [JsonRpcMethod("torrents.changeLabel")]
        public void ChangeLabel(string infoHash, string label)
        {
            _messageBus.Publish(new ChangeTorrentLabelMessage(infoHash) {Label = label});
        }

        [JsonRpcMethod("torrents.setFilePriority")]
        public void SetFilePriority(string infoHash, int fileIndex, int priority)
        {
            _messageBus.Publish(new ChangeFilePriorityMessage(infoHash, fileIndex, priority));
        }

        [JsonRpcMethod("torrents.clearError")]
        public void ClearError(string infoHash)
        {
            _messageBus.Publish(new ClearTorrentErrorMessage(infoHash));
        }

        /*
         * Queuing
        */

        [JsonRpcMethod("torrents.queue.bottom")]
        public void QueuePositionBottom(string infoHash)
        {
            _messageBus.Publish(new QueuePositionBottomMessage(infoHash));
        }

        [JsonRpcMethod("torrents.queue.down")]
        public void QueuePositionDown(string infoHash)
        {
            _messageBus.Publish(new QueuePositionDownMessage(infoHash));
        }

        [JsonRpcMethod("torrents.queue.top")]
        public void QueuePositionTop(string infoHash)
        {
            _messageBus.Publish(new QueuePositionTopMessage(infoHash));
        }

        [JsonRpcMethod("torrents.queue.up")]
        public void QueuePositionUp(string infoHash)
        {
            _messageBus.Publish(new QueuePositionUpMessage(infoHash));
        }
    }
}
