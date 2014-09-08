using System;
using Hadouken.Common.BitTorrent;
using Hadouken.Common.Extensibility.Notifications;
using Hadouken.Common.Messaging;
using Hadouken.Localization;

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
            var notif = new Notification(
                Notifications.TorrentAddedTitle,
                string.Format(Notifications.TorrentAddedMessage, message.Torrent.Name));

            _notifierHandler.Notify(notif);
        }
    }
}
