using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hadouken.Framework.Events
{
    public interface IEventService
    {
        void Subscribe<T>(string eventName, Action<T> callback);

        void Publish(string eventName, object data);
    }
}
