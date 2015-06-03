using System;
using Hadouken.Common.BitTorrent;
using Hadouken.Common.Extensibility.Notifications;
using Hadouken.Common.Messaging;
using Hadouken.Localization;

namespace Hadouken.Core.Handlers {
    public sealed class NotifyTorrentCompletedHandler : IMessageHandler<TorrentCompletedMessage> {
        private readonly INotifierEngine _notifierSender;

        public NotifyTorrentCompletedHandler(INotifierEngine notifierSender) {
            if (notifierSender == null) {
                throw new ArgumentNullException("notifierSender");
            }
            this._notifierSender = notifierSender;
        }

        public void Handle(TorrentCompletedMessage message) {
            var notif = new Notification(NotificationType.TorrentCompleted,
                Notifications.TorrentCompletedTitle,
                string.Format(Notifications.TorrentCompletedMessage, message.Torrent.Name));

            this._notifierSender.NotifyAll(notif);
        }
    }
}