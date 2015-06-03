using System;
using Hadouken.Common.Data;
using Hadouken.Common.Extensibility;
using Hadouken.Common.Extensibility.Notifications;
using Hadouken.Common.Logging;
using Hadouken.Extensions.Pushalot.Config;
using Hadouken.Extensions.Pushalot.Http;

namespace Hadouken.Extensions.Pushalot {
    [Extension("notifier.pushalot",
        Name = "Pushalot",
        Description = "Sends push notifications to Windows 8 and Windows Phone via Pushalot."
        )]
    [Configuration(typeof (PushalotConfig), Key = "pushalot.config")]
    public sealed class PushalotNotifier : INotifier {
        private readonly IKeyValueStore _keyValueStore;
        private readonly ILogger<PushalotNotifier> _logger;
        private readonly IPushalotClient _pushalotClient;

        public PushalotNotifier(ILogger<PushalotNotifier> logger,
            IPushalotClient pushalotClient,
            IKeyValueStore keyValueStore) {
            if (logger == null) {
                throw new ArgumentNullException("logger");
            }
            if (pushalotClient == null) {
                throw new ArgumentNullException("pushalotClient");
            }
            if (keyValueStore == null) {
                throw new ArgumentNullException("keyValueStore");
            }
            this._logger = logger;
            this._pushalotClient = pushalotClient;
            this._keyValueStore = keyValueStore;
        }

        public bool CanNotify() {
            var config = this._keyValueStore.Get<PushalotConfig>("pushalot.config");

            return (config != null
                    && !string.IsNullOrEmpty(config.AuthorizationToken));
        }

        public void Notify(Notification notification) {
            var config = this._keyValueStore.Get<PushalotConfig>("pushalot.config");

            if (config == null) {
                this._logger.Warn("Pushalot not configured.");
                return;
            }

            if (string.IsNullOrEmpty(config.AuthorizationToken)) {
                this._logger.Warn("Pushalot authorization token not set.");
                return;
            }

            var msg = new Message(config.AuthorizationToken, notification.Message) {
                Title = notification.Title
            };

            this._pushalotClient.Send(msg);
        }
    }
}