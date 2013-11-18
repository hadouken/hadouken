using Hadouken.Framework.Plugins;
using Hadouken.Plugins.Web.Http;

namespace Hadouken.Plugins.Web
{
    public class WebPlugin : Plugin
    {
        private readonly IHttpFileServer _httpFileServer;

        public WebPlugin(IHttpFileServer httpFileServer)
        {
            _httpFileServer = httpFileServer;
        }

        public override void OnStart()
        {
            _httpFileServer.Start();
        }

        public override void OnStop()
        {
            _httpFileServer.Stop();
        }
    }
}
