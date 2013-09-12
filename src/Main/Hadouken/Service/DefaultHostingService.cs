using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hadouken.Http;
using Hadouken.Plugins;

namespace Hadouken.Service
{
    public class DefaultHostingService : HostingService
    {
        private readonly IPluginEngine _pluginEngine;
        private readonly IHttpWebApiServer _webApiServer;

        public DefaultHostingService(IPluginEngine pluginEngine, IHttpWebApiServer webApiServer)
        {
            _pluginEngine = pluginEngine;
            _webApiServer = webApiServer;
        }

        protected override void OnStart(string[] args)
        {
            _pluginEngine.Load();

            _webApiServer.OpenAsync().Wait();
        }

        protected override void OnStop()
        {
            _webApiServer.CloseAsync().Wait();

            _pluginEngine.Unload();
        }
    }
}
