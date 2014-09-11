using System;
using System.Collections.Generic;
using System.Linq;
using Hadouken.Common.BitTorrent;
using Hadouken.Common.JsonRpc;
using Hadouken.Common.Messaging;

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

        [JsonRpcMethod("torrents.addFile")]
        public void AddFile(byte[] data, string savePath, string label)
        {
            _messageBus.Publish(new AddTorrentMessage(data) {SavePath = savePath, Label = label});
        }

        [JsonRpcMethod("torrents.addMagnetLink")]
        public void AddMagnetLink(string magnetLink, IDictionary<string, object> torrentParameters)
        {
            var msg = new AddMagnetLinkMessage(magnetLink);

            if (torrentParameters.ContainsKey("label")) msg.Label = torrentParameters["label"].ToString();
            if (torrentParameters.ContainsKey("name")) msg.Name = torrentParameters["name"].ToString();
            if (torrentParameters.ContainsKey("save_path")) msg.SavePath = torrentParameters["save_path"].ToString();

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
    }
}
