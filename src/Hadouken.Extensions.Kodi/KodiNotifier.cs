using System;
using Hadouken.Common.Extensibility;
using Hadouken.Common.Extensibility.Notifications;
using Hadouken.Extensions.Kodi.Http;

namespace Hadouken.Extensions.Kodi
{
    [Extension("notifier.kodi",
        Name = "Kodi/XBMC",
        Description = "Sends notifications to Kodi (previously XBMC).",
        ResourceNamespace = "Hadouken.Extensions.Kodi.Resources",
        Scripts = new[] { "js/app.js", "js/controllers/settingsController.js" }
    )]
    public class KodiNotifier : INotifier
    {
        private readonly IKodiClient _kodiClient;

        public KodiNotifier(IKodiClient kodiClient)
        {
            if (kodiClient == null) throw new ArgumentNullException("kodiClient");
            _kodiClient = kodiClient;
        }

        public bool CanNotify()
        {
            return true;
        }

        public void Notify(Notification notification)
        {
            _kodiClient.ShowNotification(notification.Title, notification.Message);
        }
    }
}
