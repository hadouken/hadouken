namespace Hadouken.Common.Extensibility.Notifications {
    public interface INotifier : IExtension {
        bool CanNotify();
        void Notify(Notification notification);
    }
}