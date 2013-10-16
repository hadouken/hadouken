using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hadouken.Framework.Events;
using Hadouken.Framework.Plugins;
using Hadouken.Plugins.WebUI.Nancy;
using Nancy.Hosting.Self;

namespace Hadouken.Plugins.WebUI
{
    public class WebUIPlugin : Plugin
    {
        private readonly NancyHost _nancyHost;

        public WebUIPlugin()
        {
            _nancyHost = new NancyHost(new CustomBootstrapper(), new HostConfiguration(){RewriteLocalhost = false}, new Uri("http://localhost:7890/"));
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
