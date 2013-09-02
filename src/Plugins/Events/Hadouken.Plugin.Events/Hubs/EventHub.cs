using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace Hadouken.Plugins.Events.Hubs
{
    [HubName("events")]
    public class EventHub : Hub
    {
        [HubMethodName("publish")]
        public void Publish(string eventName, object data)
        {
            Clients.All.eventPublished(new
            {
                name = eventName,
                data
            });
        }
    }
}
