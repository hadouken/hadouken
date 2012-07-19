using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hadouken.Plugins
{
    public interface IPluginEngine : IComponent
    {
        IEnumerable<IPluginManager> Refresh();

        void LoadAll();
        void UnloadAll();

        IDictionary<string, IPluginManager> Managers { get;}
    }
}
