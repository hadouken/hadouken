using Hadouken.Common.Extensibility;
using Hadouken.Common.Extensibility.Notifications;

namespace Hadouken.Extensions.Pushover
{
    [Extension("notifier.pushover",
        Name = "Pushover",
        Description = "Sends push notifications to your mobile devices via Pushover."
    )]
    public class PushoverNotifier : INotifier
    {
        public void Notify(Notification notification)
        {
            throw new System.NotImplementedException();
        }
    }
}
