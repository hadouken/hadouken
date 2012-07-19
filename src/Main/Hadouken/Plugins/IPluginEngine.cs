using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hadouken.Plugins
{
    public interface IPluginEngine : IComponent
    {
        void Refresh();
        void UnloadAll();

        IDictionary<string, IPluginManager> Managers { get;}
    }
}
