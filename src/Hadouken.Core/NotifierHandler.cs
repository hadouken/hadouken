using System;
using System.Linq;
using Hadouken.Common.Extensibility;
using Hadouken.Common.Extensibility.Notifications;

namespace Hadouken.Core
{
    public class NotifierHandler : INotifierHandler
    {
        private readonly IExtensionFactory _extensionFactory;

        public NotifierHandler(IExtensionFactory extensionFactory)
        {
            if (extensionFactory == null) throw new ArgumentNullException("extensionFactory");
            _extensionFactory = extensionFactory;
        }

        public void Notify(Notification notification)
        {
            var notifiers = _extensionFactory.GetAll<INotifier>().Where(n => _extensionFactory.IsEnabled(n.GetId()));

            foreach (var notifier in notifiers)
            {
                notifier.Notify(notification);
            }
        }
    }
}