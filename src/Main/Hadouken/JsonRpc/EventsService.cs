using Hadouken.Fx.JsonRpc;
using Microsoft.AspNet.SignalR;

namespace Hadouken.JsonRpc
{
    public class EventsService : IJsonRpcService
    {
        [JsonRpcMethod("core.events.publish")]
        public void PublishEvent(string name, object data)
        {
            var hub = GlobalHost.ConnectionManager.GetHubContext("events");
            hub.Clients.All.publishEvent(new {name, data});
        }
    }
}
