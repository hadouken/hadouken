using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hadouken.Framework.Rpc;
using Hadouken.Plugins.Events.Hubs;
using Microsoft.AspNet.SignalR.Client;

namespace Hadouken.Plugins.Events.Rpc
{
    [RpcMethod("events.publish")]
    public class EventsPublish : IRpcMethod
    {
        private readonly HubConnection _hubConnection;
        private readonly Lazy<IHubProxy> _proxy; 

        public EventsPublish(string signalrUri)
        {
            _hubConnection = new HubConnection(signalrUri, false);
            _proxy = new Lazy<IHubProxy>(CreateProxy);
        }

        public bool Execute(EventsPublishDto dto)
        {
            _proxy.Value.Invoke("publish", dto.EventName, dto.Data);

            return true;
        }

        private IHubProxy CreateProxy()
        {
            var proxy = _hubConnection.CreateHubProxy("events");
            _hubConnection.Start().Wait();

            return proxy;
        }
    }

    public class EventsPublishDto
    {
        public string EventName { get; set; }

        public object Data { get; set; }
    }
}
