using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hadouken.Framework;
using Hadouken.Framework.Events;
using Hadouken.Framework.Http;
using Hadouken.Framework.Http.Media;
using Hadouken.Framework.IO;
using Hadouken.Framework.Plugins;

namespace Hadouken.Plugins.WebUI
{
    public class WebUIBootstrapper : Bootstrapper
    {
        public override Plugin Load(IBootConfig config)
        {
            var uri = String.Format("http://{0}:{1}/", config.HostBinding, config.Port);
            var eventListenerUri = new Uri(String.Format("http://{0}:{1}/events", config.HostBinding, config.Port));
            var mediaTypeFactory = new MediaTypeFactory(new FileSystem());
            var server = new HttpFileServer(uri, Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "UI"), mediaTypeFactory, new EventListener(eventListenerUri));
            server.SetCredentials(config.UserName, config.Password);

            return new WebUIPlugin(server);
        }
    }
}
