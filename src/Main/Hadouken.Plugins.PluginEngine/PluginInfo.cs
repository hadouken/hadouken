using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hadouken.Plugins.PluginEngine
{
    public sealed class PluginInfo : IPluginInfo
    {
        public PluginInfo(string name, Version version)
        {
            Name = name;
            Version = version;
            State = PluginState.Unloaded;

            Assemblies = new List<byte[]>();
        }

        public string Name { get; private set; }

        public Version Version { get; private set; }

        public PluginState State { get; internal set; }

        public PluginSandbox Sandbox { get; set; }

        public PluginManifest Manifest { get; set; }

        public List<byte[]> Assemblies { get; private set; } 
    }
}
