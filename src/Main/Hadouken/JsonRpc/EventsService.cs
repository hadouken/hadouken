using System;
using System.Linq;
using Hadouken.Fx.JsonRpc;
using Hadouken.Plugins;
using Serilog;

namespace Hadouken.JsonRpc
{
    public class EventsService : IJsonRpcService
    {
        private const string All = "<all>";
        private readonly ILogger _logger;
        private readonly IPluginEngine _pluginEngine;
        private readonly IJsonRpcClient _rpcClient;

        public EventsService(ILogger logger, IPluginEngine pluginEngine, IJsonRpcClient rpcClient)
        {
            _logger = logger;
            _pluginEngine = pluginEngine;
            _rpcClient = rpcClient;
        }

        [JsonRpcMethod("core.events.publish")]
        public void PublishEvent(string name, object data)
        {
            // Get all plugins with event handlers for the current event
            // or for <all> events.
            var handlers = _pluginEngine.GetAll()
                .Where(p => p.State == PluginState.Loaded)
                .Select(p => p.Package.GetManifest())
                .SelectMany(
                    m => m.EventHandlers.Where(e => e.Name == name || e.Name == All))
                .ToList();

            _logger.Debug("Publishing event {EventName} to {Count} handlers.", name, handlers.Count());

            foreach (var handler in handlers)
            {
                // These are the handlers for our event
                try
                {
                    _logger.Debug("Calling {Target} for event {EventName}.", handler.Target, name);

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
                    _logger.Error(e, "Error when calling event handler {EventHandler}", handler);
                }
            }
        }
    }
}
