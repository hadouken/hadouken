using System;
using Hadouken.Common.BitTorrent;
using Hadouken.Common.Extensibility.Notifications;
using Hadouken.Common.Messaging;

namespace Hadouken.Core.Handlers
{
    public sealed class NotifyTorrentAddedHandler : IMessageHandler<TorrentAddedMessage>
    {
        private readonly INotifierHandler _notifierHandler;

        public NotifyTorrentAddedHandler(INotifierHandler notifierHandler)
        {
            if (notifierHandler == null) throw new ArgumentNullException("notifierHandler");
            _notifierHandler = notifierHandler;
        }

        public void Handle(TorrentAddedMessage message)
        {
            var notif = new Notification("Torrent added",
                string.Format("Torrent {0} added To Hadouken.", message.Torrent.Name));

            _notifierHandler.Notify(notif);
        }
    }
}
