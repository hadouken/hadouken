using System;
using Hadouken.Common.Data;
using Hadouken.Common.Extensibility;
using Hadouken.Common.Extensibility.Notifications;
using Hadouken.Extensions.Kodi.Config;
using Hadouken.Extensions.Kodi.Http;

namespace Hadouken.Extensions.Kodi
{
    [Extension("notifier.kodi",
        Name = "Kodi/XBMC",
        Description = "Sends notifications to Kodi (previously XBMC)."
    )]
    [Configuration(typeof(KodiConfig), Key = "kodi.config")]
    public class KodiNotifier : INotifier
    {
        private readonly IKodiClient _kodiClient;
        private readonly IKeyValueStore _keyValueStore;

        public KodiNotifier(IKodiClient kodiClient,
            IKeyValueStore keyValueStore)
        {
            if (kodiClient == null) throw new ArgumentNullException("kodiClient");
            if (keyValueStore == null) throw new ArgumentNullException("keyValueStore");
            _kodiClient = kodiClient;
            _keyValueStore = keyValueStore;
        }

        public bool CanNotify()
        {
            var config = _keyValueStore.Get<KodiConfig>("kodi.config");

            if (config == null) return false;
            if (config.Url == null) return false;

            return !config.EnableAuthentication
                   || (!string.IsNullOrEmpty(config.UserName)
                       && !string.IsNullOrEmpty(config.Password));
        }

        public void Notify(Notification notification)
        {
            if (notification == null) throw new ArgumentNullException("notification");

            _kodiClient.ShowNotification(notification.Title, notification.Message);
        }
    }
}
