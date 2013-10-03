using System;
using System.Collections.Generic;
using Microsoft.AspNet.SignalR.Client.Hubs;
using NLog;

namespace Hadouken.Framework.Events
{
    public class EventListener : IEventListener
    {
        private static class CallbackManager<T>
        {
            private static readonly IDictionary<string, IList<Action<T>>> Callbacks = new Dictionary<string, IList<Action<T>>>();

            public static IList<Action<T>> GetCallbacks(string key)
            {
                if (!Callbacks.ContainsKey(key))
                    Callbacks.Add(key, new List<Action<T>>());

                return Callbacks[key];
            }
        }

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly Uri _host;
        private readonly Lazy<HubConnection> _connection; 
        private readonly Lazy<IHubProxy> _proxy;

        public EventListener(Uri host)
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
            var proxy = _connection.Value.CreateHubProxy("events");
            _connection.Value.Start();

            return proxy;
        }

        public void Subscribe<T>(string eventName, Action<T> callback)
        {
            if (!_proxy.IsValueCreated)
                _proxy.Value.On<string, object>("publishEvent", EventCallback);

            CallbackManager<T>.GetCallbacks(eventName).Add(callback);
        }

        private void EventCallback(string name, object data)
        {
            Logger.Debug("Event received: {0}", name);
        }
    }
}
