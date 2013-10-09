using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hadouken.Framework;
using Hadouken.Framework.Http;
using Hadouken.Framework.Plugins;

namespace Hadouken.Plugins.WebUI
{
    public class WebUIBootstrapper : Bootstrapper
    {
        public override Plugin Load(IBootConfig config)
        {
            var uri = String.Format("http://{0}:{1}/", config.HostBinding, config.Port);
            var server = new HttpFileServer(uri, Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "UI"));
            server.SetCredentials(config.UserName, config.Password);

            return new WebUIPlugin(server);
        }
    }
}
