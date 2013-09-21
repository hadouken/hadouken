using System;
using System.IO;
using Hadouken.Framework;
using Hadouken.Framework.Http;
using Hadouken.Framework.Plugins;

namespace Hadouken.Plugins.Torrents
{
    public class TorrentsBootstrapper : Bootstrapper
    {
        public override Plugin Load(IBootConfig config)
        {
            var uri = String.Format("http://{0}:{1}/plugins/core.torrents/", config.HostBinding, config.Port);
            var server = new HttpFileServer(uri, Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "UI"),
                "/plugins/core.torrents/");

            return new TorrentsPlugin(server);
        }
    }
}
