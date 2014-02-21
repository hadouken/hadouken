namespace Hadouken.Configuration.AppConfig
{
    public class PluginElementWrapper : IPluginConfiguration
    {
        private readonly PluginElement _pluginElement;

        public PluginElementWrapper(PluginElement pluginElement)
        {
            _pluginElement = pluginElement;
        }

        public string Id
        {
            get { return _pluginElement.Id; }
        }

        public string Path
        {
            get { return _pluginElement.Path; }
        }
    }
}
