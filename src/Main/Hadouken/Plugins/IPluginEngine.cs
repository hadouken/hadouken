using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hadouken.Plugins
{
    public interface IPluginEngine
    {
        void Load();
        void Load(string path);

        void UnloadAll();

        IDictionary<string, IPluginManager> Managers { get;}
    }
}
