using System.Collections.Generic;
using Hadouken.Common.Extensibility.Notifications;

namespace Hadouken.Core
{
    public interface INotifierEngine
    {
        IEnumerable<INotifier> GetAll(); 

        void NotifyAll(Notification notification);
    }
}
