using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hadouken.Plugins
{
    public interface IPluginManager
    {
        string Name { get; }

        Version Version { get; }

        void Load();

        void Unload();
    }
}
