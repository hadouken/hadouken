using Hadouken.Framework.Plugins;
using Hadouken.Plugins.Config.Data;

namespace Hadouken.Plugins.Config
{
    public class ConfigPlugin : Plugin
    {
        private readonly IConfigDataStore _dataStore;

        public ConfigPlugin(IConfigDataStore dataStore)
        {
            _dataStore = dataStore;
        }

        public override void OnStart()
        {
        }

        public override void OnStop()
        {
            _dataStore.Save();
        }
    }
}
