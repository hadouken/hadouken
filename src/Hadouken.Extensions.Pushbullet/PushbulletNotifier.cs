using System;
using Hadouken.Common.Data;
using Hadouken.Common.Extensibility;
using Hadouken.Common.Extensibility.Notifications;
using Hadouken.Common.Logging;
using Hadouken.Extensions.Pushbullet.Config;
using Hadouken.Extensions.Pushbullet.Http;

namespace Hadouken.Extensions.Pushbullet
{
    [Extension("notifier.pushbullet",
        Name = "Pushbullet",
        Description = "Sends push notifications to your devices via Pushbullet.",
        ResourceNamespace = "Hadouken.Extensions.Pushbullet.Resources",
        Scripts = new[] { "js/app.js", "js/controllers/settingsController.js" }
    )]
    public class PushbulletNotifier : INotifier
    {
        private readonly ILogger<PushbulletNotifier> _logger;
        private readonly IKeyValueStore _keyValueStore;
        private readonly IPushbulletClient _pushbulletClient;

        public PushbulletNotifier(ILogger<PushbulletNotifier> logger,
            IKeyValueStore keyValueStore,
            IPushbulletClient pushbulletClient)
        {
            if (logger == null) throw new ArgumentNullException("logger");
            if (keyValueStore == null) throw new ArgumentNullException("keyValueStore");
            if (pushbulletClient == null) throw new ArgumentNullException("pushbulletClient");

            _logger = logger;
            _keyValueStore = keyValueStore;
            _pushbulletClient = pushbulletClient;
        }

        public bool CanNotify()
        {
            var config = _keyValueStore.Get<PushbulletConfig>("pushbullet.config");

            return (config != null
                    && !string.IsNullOrEmpty(config.AccessToken));
        }

        public void Notify(Notification notification)
        {
            var config = _keyValueStore.Get<PushbulletConfig>("pushbullet.config");

            if (config == null)
            {
                _logger.Warn("Pushbullet not configured.");
                return;
            }

            if (string.IsNullOrEmpty(config.AccessToken))
            {
                _logger.Warn("Pushbullet access token not set.");
                return;
            }

            _pushbulletClient.Send(config.AccessToken, new Note(notification.Title, notification.Message));
        }
    }
}
