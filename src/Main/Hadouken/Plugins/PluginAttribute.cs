using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hadouken.Plugins
{
    [AttributeUsage(AttributeTargets.Class)]
    public class PluginAttribute : Attribute
    {
        public PluginAttribute(string name, string version)
        {
            Name = name;
            Version = new Version(version);
        }

        public string Name { get; set; }

        public Version Version { get; set; }

        public string ResourceBase { get; set; }
    }
}
