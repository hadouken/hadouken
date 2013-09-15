using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hadouken.Framework.Rpc;
using Hadouken.Plugins.Events.Hubs;

namespace Hadouken.Plugins.Events.Rpc
{
    public class EventsService : IJsonRpcService
    {
        private readonly IEventHub _eventHub;

        public EventsService(IEventHub eventHub)
        {
            _eventHub = eventHub;
        }

        [JsonRpcMethod("events.publish")]
        public bool Publish(string name, object data)
        {
            _eventHub.Publish(name, data);
            return true;
        }
    }
}
