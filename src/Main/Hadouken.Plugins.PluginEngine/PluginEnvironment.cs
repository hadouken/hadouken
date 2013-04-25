using Hadouken.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace Hadouken.Plugins.PluginEngine
{
    public class PluginEnvironment : IEnvironment
    {
        public string ConnectionString { get; set; }
        public NetworkCredential HttpCredentials { get; set; }
        public Uri HttpBinding { get; set; }
    }
}
