using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hadouken.Plugins
{
    public sealed class PluginInfo
    {
        internal PluginInfo(string name, Version version)
        {
            Name = name;
            Version = version;
        }

        public string Name { get; private set; }

        public Version Version { get; private set; }
    }
}
