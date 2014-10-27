using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using Hadouken.Common.BitTorrent;
using Hadouken.Common.Messaging;
using Hadouken.Common.Text;
using Hadouken.Core.Http.WebSockets;

namespace Hadouken.Core.Http
{
    public class EventStreamServer : WebSocketConnection
    {
        private readonly IMessageBus _messageBus;
        private readonly IJsonSerializer _jsonSerializer;

        public EventStreamServer(IMessageBus messageBus,
            IJsonSerializer jsonSerializer)
        {
            if (messageBus == null) throw new ArgumentNullException("messageBus");
            if (jsonSerializer == null) throw new ArgumentNullException("jsonSerializer");
            _messageBus = messageBus;
            _jsonSerializer = jsonSerializer;
        }

        public override void OnOpen()
        {
            _messageBus.Subscribe<TorrentAddedMessage>(OnTorrentAdded);
            _messageBus.Subscribe<TorrentCompletedMessage>(OnTorrentCompleted);
            _messageBus.Subscribe<TorrentRemovedMessage>(OnTorrentRemoved);
        }

        public override void OnClose(WebSocketCloseStatus closeStatus, string closeDescription)
        {
            _messageBus.Unsubscribe<TorrentAddedMessage>(OnTorrentAdded);
            _messageBus.Unsubscribe<TorrentCompletedMessage>(OnTorrentCompleted);
            _messageBus.Unsubscribe<TorrentRemovedMessage>(OnTorrentRemoved);
        }

        private void OnTorrentAdded(TorrentAddedMessage message)
        {
            SendEvent("torrent.added", message.Torrent);
        }

        private void OnTorrentCompleted(TorrentCompletedMessage message)
        {
            SendEvent("torrent.completed", message.Torrent);
        }

        private void OnTorrentRemoved(TorrentRemovedMessage message)
        {
            SendEvent("torrent.removed", message.InfoHash);
        }

        private void SendEvent(string type, object payload)
        {
            var evnt = new Dictionary<string, object>();
            evnt.Add("type", type);
            evnt.Add(type, payload);

            var json = _jsonSerializer.SerializeObject(evnt);
            var data = Encoding.UTF8.GetBytes(json);

            SendAsyncText(data, true);
        }
    }
}
