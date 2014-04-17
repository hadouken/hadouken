using Hadouken.Http.Management.Security;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace Hadouken.Http.Management.Hubs
{
    [CustomClientAuthorize]
    [HubName("events")]
    public class EventHub : Hub
    {
        public void PublishEvent(string name, object data)
        {
            Clients.All.publishEvent(name, data);
        }
    }
}
