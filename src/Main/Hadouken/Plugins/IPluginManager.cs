using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hadouken.Plugins
{
    public interface IPluginManager
    {
        void Load();
        void Unload();

        void Install();
        void Uninstall();

        string Name { get; }
        Version Version { get; }
        string ResourceBase { get; }
    }
}
