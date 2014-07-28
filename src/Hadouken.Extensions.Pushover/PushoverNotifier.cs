using System;
using Hadouken.Common.Data;
using Hadouken.Common.Extensibility;
using Hadouken.Common.Extensibility.Notifications;
using Hadouken.Common.Logging;
using Hadouken.Extensions.Pushover.Config;
using Hadouken.Extensions.Pushover.Http;

namespace Hadouken.Extensions.Pushover
{
    [Extension("notifier.pushover",
        Name = "Pushover",
        Description = "Sends push notifications to your mobile devices via Pushover.",
        ResourceNamespace = "Hadouken.Extensions.Pushover.Resources",
        Scripts = new [] { "js/app.js", "js/controllers/settingsController.js" }
    )]
    public class PushoverNotifier : INotifier
    {
        private readonly ILogger _logger;
        private readonly IKeyValueStore _keyValueStore;
        private readonly IPushoverClient _pushoverClient;

        public PushoverNotifier(ILogger logger,
            IKeyValueStore keyValueStore,
            IPushoverClient pushoverClient)
        {
            if (logger == null) throw new ArgumentNullException("logger");
            if (keyValueStore == null) throw new ArgumentNullException("keyValueStore");
            if (pushoverClient == null) throw new ArgumentNullException("pushoverClient");

            _logger = logger;
            _keyValueStore = keyValueStore;
            _pushoverClient = pushoverClient;
        }

        public void Notify(Notification notification)
        {
            var config = _keyValueStore.Get<PushoverConfig>("pushover.config");
            
            if (config == null)
            {
                _logger.Warn("Pushover not configured.");
                return;
            }

            var message = new PushoverMessage(config.AppKey, config.UserKey, notification.Message)
            {
                Title = notification.Title
            };

            _pushoverClient.Send(message);
        }
    }
}
