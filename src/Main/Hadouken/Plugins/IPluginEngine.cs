using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hadouken.Plugins
{
    public interface IPluginEngine
    {
        IEnumerable<IPluginManager> PluginManagers { get; } 

        void Load();

        void Unload();
    }
}
