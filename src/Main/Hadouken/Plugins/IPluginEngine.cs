using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hadouken.Plugins
{
    public interface IPluginEngine
    {
        IEnumerable<IPluginManager> GetAll();

        IPluginManager Get(string name);

        void Load();

        void Unload();
    }
}
