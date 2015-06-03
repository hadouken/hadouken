using System;
using Hadouken.Common.BitTorrent;
using Hadouken.Common.Extensibility.Notifications;
using Hadouken.Common.Messaging;
using Hadouken.Localization;

namespace Hadouken.Core.Handlers {
    public sealed class NotifyTorrentAddedHandler : IMessageHandler<TorrentAddedMessage> {
        private readonly INotifierEngine _notifierSender;

        public NotifyTorrentAddedHandler(INotifierEngine notifierSender) {
            if (notifierSender == null) {
                throw new ArgumentNullException("notifierSender");
            }
            this._notifierSender = notifierSender;
        }

        public void Handle(TorrentAddedMessage message) {
            var notif = new Notification(NotificationType.TorrentAdded,
                Notifications.TorrentAddedTitle,
                string.Format(Notifications.TorrentAddedMessage, message.Torrent.Name));

            this._notifierSender.NotifyAll(notif);
        }
    }
}