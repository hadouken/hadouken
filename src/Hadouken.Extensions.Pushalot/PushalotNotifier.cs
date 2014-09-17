using System;
using Hadouken.Common.Data;
using Hadouken.Common.Extensibility;
using Hadouken.Common.Extensibility.Notifications;
using Hadouken.Common.Logging;
using Hadouken.Extensions.Pushalot.Config;
using Hadouken.Extensions.Pushalot.Http;

namespace Hadouken.Extensions.Pushalot
{
    [Extension("notifier.pushalot",
        Name = "Pushalot",
        Description = "Sends push notifications to Windows 8 and Windows Phone via Pushalot.",
        ResourceNamespace = "Hadouken.Extensions.Pushalot.Resources",
        Scripts = new[] { "js/app.js", "js/controllers/settingsController.js" }
    )]
    public sealed class PushalotNotifier : INotifier
    {
        private readonly ILogger<PushalotNotifier> _logger;
        private readonly IPushalotClient _pushalotClient;
        private readonly IKeyValueStore _keyValueStore;

        public PushalotNotifier(ILogger<PushalotNotifier> logger, IPushalotClient pushalotClient, IKeyValueStore keyValueStore)
        {
            if (logger == null) throw new ArgumentNullException("logger");
            if (pushalotClient == null) throw new ArgumentNullException("pushalotClient");
            if (keyValueStore == null) throw new ArgumentNullException("keyValueStore");
            _logger = logger;
            _pushalotClient = pushalotClient;
            _keyValueStore = keyValueStore;
        }

        public bool CanNotify()
        {
            var config = _keyValueStore.Get<PushalotConfig>("pushalot.config");

            return (config != null
                    && !string.IsNullOrEmpty(config.AuthorizationToken));
        }

        public void Notify(Notification notification)
        {
            var config = _keyValueStore.Get<PushalotConfig>("pushalot.config");

            if (config == null)
            {
                _logger.Warn("Pushalot not configured.");
                return;
            }

            if (string.IsNullOrEmpty(config.AuthorizationToken))
            {
                _logger.Warn("Pushalot authorization token not set.");
                return;
            }

            var msg = new Message(config.AuthorizationToken, notification.Message)
            {
                Title = notification.Title
            };

            _pushalotClient.Send(msg);
        }
    }
}
