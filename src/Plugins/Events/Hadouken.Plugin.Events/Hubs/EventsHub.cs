using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace Hadouken.Plugins.Events.Hubs
{
    [HubName("events")]
    public class EventsHub : Hub
    {
    }
}
