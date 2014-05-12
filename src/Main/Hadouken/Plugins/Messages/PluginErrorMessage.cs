using Hadouken.Messaging;

namespace Hadouken.Plugins.Messages
{
    public class PluginErrorMessage : Message
    {
        private readonly string _pluginId;

        public PluginErrorMessage(string pluginId)
        {
            _pluginId = pluginId;
        }

        public string PluginId
        {
            get { return _pluginId; }
        }
    }
}
