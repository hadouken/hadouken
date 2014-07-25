namespace Hadouken.Common.Extensibility.Notifications
{
    public interface INotifier : IExtension
    {
        void Notify(Notification notification);
    }
}
