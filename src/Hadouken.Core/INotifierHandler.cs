using Hadouken.Common.Extensibility.Notifications;

namespace Hadouken.Core
{
    public interface INotifierHandler
    {
        void Notify(Notification notification);
    }
}
