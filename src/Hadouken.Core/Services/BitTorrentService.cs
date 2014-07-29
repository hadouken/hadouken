using System;
using System.Collections.Generic;
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
    }
}
