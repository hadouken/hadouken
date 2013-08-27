using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Client.Hubs;

namespace Hadouken.Framework.Events
{
    public class EventService : IEventService
    {
        private readonly Uri _host;
        private readonly Lazy<HubConnection> _connection; 
        private readonly Lazy<IHubProxy> _proxy;

        public EventService(Uri host)
        {
            _host = host;
            _connection = new Lazy<HubConnection>(CreateConnection);
            _proxy = new Lazy<IHubProxy>(CreateProxy);
        }

        private HubConnection CreateConnection()
        {
            return new HubConnection(_host.ToString());
        }

        private IHubProxy CreateProxy()
        {
            var proxy = _connection.Value.CreateHubProxy("signalr");
            _connection.Value.Start();

            return proxy;
        }

        public void Subscribe<T>(string eventName, Action<T> callback)
        {
            _proxy.Value.On(eventName, callback);
        }

        public Task Publish(string eventName, object data)
        {
            return _proxy.Value.Invoke("Publish", new {eventName, data});
        }
    }
}
