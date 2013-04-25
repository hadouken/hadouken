using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hadouken.Plugins
{
    public interface IPluginInfo
    {
        string Name { get; }
        Version Version { get; }
        PluginState State { get; }
    }
}
