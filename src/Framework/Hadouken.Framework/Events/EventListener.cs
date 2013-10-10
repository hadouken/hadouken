using System;
using System.Collections.Generic;
using Microsoft.AspNet.SignalR.Client.Hubs;
using Newtonsoft.Json.Linq;
using NLog;

namespace Hadouken.Framework.Events
{
    public class EventListener : IEventListener
    {
        private static class CallbackManager
        {
            private static readonly IDictionary<string, IList<Action<JToken>>> Callbacks = new Dictionary<string, IList<Action<JToken>>>();

            public static IList<Action<JToken>> GetCallbacks(string key)
            {
                if (!Callbacks.ContainsKey(key))
                    Callbacks.Add(key, new List<Action<JToken>>());

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
                _proxy.Value.On<JToken>("publishEvent", EventCallback);

            Action<JToken> wrapper = d =>
            {
                var args = ConvertTokenToType<T>(d);
                callback(args);
            };

            CallbackManager.GetCallbacks(eventName).Add(wrapper);
        }

        private T ConvertTokenToType<T>(JToken token)
        {
            return token["data"].ToObject<T>();
        }

        private void EventCallback(JToken eventData)
        {
            var eventName = eventData["name"].Value<string>();
            var handlers = CallbackManager.GetCallbacks(eventName);

            foreach (var handler in handlers)
            {
                handler(eventData);
            }
        }
    }
}
