using System;
using Hadouken.Framework.Rpc;
using Hadouken.Plugins.Events.Hubs;
using Microsoft.AspNet.SignalR;

namespace Hadouken.Plugins.Events.Rpc
{
    public class EventsService : IJsonRpcService
    {
        private static readonly Lazy<IHubContext> HubInstance = new Lazy<IHubContext>(
            () => GlobalHost.ConnectionManager.GetHubContext<EventsHub>());

        [JsonRpcMethod("events.publish")]
        public bool Publish(string name, object data)
        {
            HubInstance.Value.Clients.All.eventPublished(name, data);

            return true;
        }
    }
}
