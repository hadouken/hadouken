using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hadouken.Framework.Wcf;

namespace Hadouken.Framework.Plugins
{
    public class PluginHost : IPluginHost
    {
        private readonly Plugin _plugin;
        private readonly IServiceHost _managementService;

        public PluginHost(IServiceHostFactory<IPluginManagerService> serviceHostFactory, IBootConfig bootConfig, Plugin plugin)
        {
            _plugin = plugin;
            _managementService = serviceHostFactory.Create(new Uri(bootConfig.RpcPluginUri));
        }

        public void Load()
        {
            _managementService.Open();
            _plugin.OnStart();
        }

        public void Unload()
        {
            _plugin.OnStop();
            _managementService.Close();
        }
    }
}
