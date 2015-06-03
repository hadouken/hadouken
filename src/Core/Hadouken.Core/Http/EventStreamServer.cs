using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using Hadouken.Common.BitTorrent;
using Hadouken.Common.Messaging;
using Hadouken.Common.Text;
using Hadouken.Core.Http.WebSockets;

namespace Hadouken.Core.Http {
    public class EventStreamServer : WebSocketConnection {
        private readonly IJsonSerializer _jsonSerializer;
        private readonly IMessageBus _messageBus;

        public EventStreamServer(IMessageBus messageBus,
            IJsonSerializer jsonSerializer) {
            if (messageBus == null) {
                throw new ArgumentNullException("messageBus");
            }
            if (jsonSerializer == null) {
                throw new ArgumentNullException("jsonSerializer");
            }
            this._messageBus = messageBus;
            this._jsonSerializer = jsonSerializer;
        }

        public override void OnOpen() {
            this._messageBus.Subscribe<TorrentAddedMessage>(this.OnTorrentAdded);
            this._messageBus.Subscribe<TorrentCompletedMessage>(this.OnTorrentCompleted);
            this._messageBus.Subscribe<TorrentRemovedMessage>(this.OnTorrentRemoved);
        }

        public override void OnClose(WebSocketCloseStatus closeStatus, string closeDescription) {
            this._messageBus.Unsubscribe<TorrentAddedMessage>(this.OnTorrentAdded);
            this._messageBus.Unsubscribe<TorrentCompletedMessage>(this.OnTorrentCompleted);
            this._messageBus.Unsubscribe<TorrentRemovedMessage>(this.OnTorrentRemoved);
        }

        private void OnTorrentAdded(TorrentAddedMessage message) {
            this.SendEvent("torrent.added", message.Torrent);
        }

        private void OnTorrentCompleted(TorrentCompletedMessage message) {
            this.SendEvent("torrent.completed", message.Torrent);
        }

        private void OnTorrentRemoved(TorrentRemovedMessage message) {
            this.SendEvent("torrent.removed", message.InfoHash);
        }

        private void SendEvent(string type, object payload) {
            var evnt = new Dictionary<string, object>();
            evnt.Add("type", type);
            evnt.Add(type, payload);

            var json = this._jsonSerializer.SerializeObject(evnt);
            var data = Encoding.UTF8.GetBytes(json);

            this.SendAsyncText(data, true);
        }
    }
}