using System;
using Hadouken.Common.BitTorrent;
using Hadouken.Common.Extensibility.Notifications;
using Hadouken.Common.Messaging;
using Hadouken.Localization;

namespace Hadouken.Core.Handlers
{
    public sealed class NotifyTorrentCompletedHandler : IMessageHandler<TorrentCompletedMessage>
    {
        private readonly INotifierHandler _notifierHandler;

        public NotifyTorrentCompletedHandler(INotifierHandler notifierHandler)
        {
            if (notifierHandler == null) throw new ArgumentNullException("notifierHandler");
            _notifierHandler = notifierHandler;
        }

        public void Handle(TorrentCompletedMessage message)
        {
            var notif = new Notification(
                Notifications.TorrentCompletedTitle,
                string.Format(Notifications.TorrentCompletedMessage, message.Torrent.Name));

            _notifierHandler.Notify(notif);
        }
    }
}
