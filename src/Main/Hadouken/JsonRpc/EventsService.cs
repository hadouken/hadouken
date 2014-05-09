using System;
using System.Linq;
using Hadouken.Fx.JsonRpc;
using Hadouken.Plugins;
using Microsoft.AspNet.SignalR;
using NLog;

namespace Hadouken.JsonRpc
{
    public class EventsService : IJsonRpcService
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private const string All = "<all>";
        private readonly IPluginEngine _pluginEngine;
        private readonly IJsonRpcClient _rpcClient;

        public EventsService(IPluginEngine pluginEngine, IJsonRpcClient rpcClient)
        {
            _pluginEngine = pluginEngine;
            _rpcClient = rpcClient;
        }

        [JsonRpcMethod("core.events.publish")]
        public void PublishEvent(string name, object data)
        {
            // Get all plugins with event handlers for the current event
            // or for <all> events.
            var handlers = _pluginEngine.GetAll()
                .SelectMany(
                    p => p.Manifest.EventHandlers.Where(e => e.Name == name || e.Name == All));

            foreach (var handler in handlers)
            {
                // These are the handlers for our event
                try
                {
                    if (handler.Name == All)
                    {
                        _rpcClient.Call(handler.Target, new[] {name, data});
                    }
                    else
                    {
                        _rpcClient.Call(handler.Target, new[] { data });
                    }
                }
                catch (Exception e)
                {
                    Logger.ErrorException("Error when calling event handler: " + handler, e);
                }
            }

            // To keep legacy plugins from not receiving events.
            var hub = GlobalHost.ConnectionManager.GetHubContext("events");
            hub.Clients.All.publishEvent(new {name, data});
        }
    }
}
