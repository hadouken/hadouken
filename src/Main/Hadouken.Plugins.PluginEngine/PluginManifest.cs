using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hadouken.Plugins.PluginEngine
{
    [Serializable]
    public class PluginManifest
    {
        public string Name { get; set; }
        public Version Version { get; set; }
    }
}
