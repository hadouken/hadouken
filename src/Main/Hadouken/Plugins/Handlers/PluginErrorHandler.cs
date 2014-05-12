using System.Linq;
using Hadouken.Messaging;
using Hadouken.Plugins.Messages;
using NLog;

namespace Hadouken.Plugins.Handlers
{
    public class PluginErrorHandler : IMessageHandler<PluginErrorMessage>
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly IPluginEngine _pluginEngine;

        public PluginErrorHandler(IPluginEngine pluginEngine)
        {
            _pluginEngine = pluginEngine;
        }

        public void Handle(PluginErrorMessage message)
        {
            Logger.Trace("Plugin {0} crashed. Reviving.", message.PluginId);

            var failedPlugin = _pluginEngine.Get(message.PluginId);

            if (failedPlugin == null) return;

            var unloadOrder = _pluginEngine.GetUnloadOrder(message.PluginId);

            foreach (var pluginId in unloadOrder)
            {
                _pluginEngine.Unload(pluginId);
            }

            foreach (var pluginId in unloadOrder.Reverse())
            {
                _pluginEngine.Load(pluginId);
            }
        }
    }
}
