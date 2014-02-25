using Hadouken.Fx.ServiceModel;

namespace Hadouken.Fx
{
    public sealed class PluginHost : IPluginHost
    {
        private readonly IPluginServiceHost _pluginService;
        private readonly Plugin _plugin;

        public PluginHost(IPluginServiceHost pluginService, Plugin plugin)
        {
            _pluginService = pluginService;
            _plugin = plugin;
        }

        public void Load()
        {
            _pluginService.Open();
            _plugin.Load();
        }

        public void Unload()
        {
            _plugin.Unload();
            _pluginService.Close();
        }
    }
}