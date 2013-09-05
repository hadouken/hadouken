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
        private readonly string _signalrUri;

        public EventsPublish(string signalrUri)
        {
            _signalrUri = signalrUri;
        }

        public void Execute(string eventName, object data)
        {
            using (var connection = new HubConnection(_signalrUri))
            {
                connection.Start();

                var proxy = connection.CreateHubProxy("events");
                proxy.Invoke("publish", eventName, data).Wait();
            }
        }
    }
}
