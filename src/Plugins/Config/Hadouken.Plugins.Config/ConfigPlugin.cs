using System;
using System.ServiceModel;
using Hadouken.Framework.Plugins;
using Hadouken.Framework.Rpc.Hosting;
using Hadouken.Framework.Wcf;

namespace Hadouken.Plugins.Config
{
    public class ConfigPlugin : Plugin
    {
        private readonly IServiceHost _rpcServer;

        public ConfigPlugin(IServiceHostFactory<IWcfRpcService> serviceHostFactory)
        {
            _rpcServer =
                serviceHostFactory.Create(new Uri("net.pipe://localhost/hdkn.plugins.core.config"));
        }

        public override void OnStart()
        {
            _rpcServer.Open();
        }

        public override void OnStop()
        {
            _rpcServer.Close();
        }
    }
}
