using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hadouken.Framework.Http;
using Hadouken.Framework.Plugins;

namespace Hadouken.Plugins.WebUI
{
    public class WebUIPlugin : Plugin
    {
        private readonly IHttpFileServer _fileServer;

        public WebUIPlugin(IHttpFileServer fileServer)
        {
            _fileServer = fileServer;
        }

        public override void Load()
        {
            _fileServer.Open();
        }

        public override void Unload()
        {
            _fileServer.Close();
        }
    }
}
