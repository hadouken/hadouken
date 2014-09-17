using System.Collections.Generic;
using Hadouken.Common.Extensibility.Notifications;

namespace Hadouken.Core.Data
{
    public interface INotifierRepository
    {
        IEnumerable<string> GetNotifiersForType(NotificationType type);

        IEnumerable<string> GetTypesForNotifier(string notifierId);

        void RegisterNotifier(string notifierId, NotificationType type);

        void UnregisterNotifier(string notifierId, NotificationType type);
    }
}
