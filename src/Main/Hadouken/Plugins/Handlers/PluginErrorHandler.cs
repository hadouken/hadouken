using System.Linq;
using Hadouken.Messaging;
using Hadouken.Plugins.Messages;
using Serilog;

namespace Hadouken.Plugins.Handlers
{
    public class PluginErrorHandler : IMessageHandler<PluginErrorMessage>
    {
        private static readonly int MaxErrorCount = 5;
        private readonly ILogger _logger;
        private readonly IPluginEngine _pluginEngine;

        public PluginErrorHandler(ILogger logger, IPluginEngine pluginEngine)
        {
            _logger = logger;
            _pluginEngine = pluginEngine;
        }

        public void Handle(PluginErrorMessage message)
        {
            var failedPlugin = _pluginEngine.Get(message.PluginId);

            if (failedPlugin == null) return;

            var unloadOrder = _pluginEngine.GetUnloadOrder(message.PluginId);

            foreach (var pluginId in unloadOrder)
            {
                _pluginEngine.Unload(pluginId);
            }

            if (failedPlugin.ErrorCount >= MaxErrorCount)
            {
                _logger.Error("Plugin {PluginId} has failed {ErrorCount} or more times. Not reloading.", message.PluginId, MaxErrorCount);
                return;
            }

            foreach (var pluginId in unloadOrder.Reverse())
            {
                _pluginEngine.Load(pluginId);
            }
        }
    }
}
