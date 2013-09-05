using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hadouken.Plugins.Events.Hubs
{
    public interface IEventHub
    {
        void Publish(string eventName, object data);
    }
}
