using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hadouken.Framework;

namespace Hadouken.Plugins
{
    public interface IPluginManager
    {
        string Name { get; }

        Version Version { get; }

        PluginState State { get; }

        void SetBootConfig(IBootConfig bootConfig);

        void Load();

        void Unload();
    }
}
