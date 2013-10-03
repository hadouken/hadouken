using Hadouken.Framework.Rpc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;

namespace Hadouken.Events.Rpc
{
    public class EventsService : IJsonRpcService
    {
        [JsonRpcMethod("events.publish")]
        public bool Publish(string name, object data)
        {
            var hub = GlobalHost.ConnectionManager.GetHubContext<EventsHub>();
            hub.Clients.All.publishEvent(new {name, data});

            return true;
        }
    }
}
