using System;
using Hadouken.Framework.Plugins;
using Hadouken.Framework.Rpc;
using Nancy.Hosting.Self;

namespace Hadouken.Plugins.Web
{
    public class WebPlugin : Plugin
    {
        private readonly NancyHost _nancyHost;

        public WebPlugin(IJsonRpcClient rpcClient)
        {
            _nancyHost = new NancyHost(new CustomNancyBootstrapper(rpcClient), new HostConfiguration(){RewriteLocalhost = false}, new Uri("http://localhost:7890/"));
        }

        public override void OnStart()
        {
            _nancyHost.Start();
        }

        public override void OnStop()
        {
            _nancyHost.Stop();
        }
    }
}
