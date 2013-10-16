using System;
using Hadouken.Framework.Plugins;
using Nancy.Hosting.Self;

namespace Hadouken.Plugins.WebUI
{
    public class WebPlugin : Plugin
    {
        private readonly NancyHost _nancyHost;

        public WebPlugin()
        {
            _nancyHost = new NancyHost(new CustomNancyBootstrapper(), new HostConfiguration(){RewriteLocalhost = false}, new Uri("http://localhost:7890/"));
        }

        public override void Load()
        {
            _nancyHost.Start();
        }

        public override void Unload()
        {
            _nancyHost.Stop();
        }
    }
}
