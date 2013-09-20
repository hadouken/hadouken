using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hadouken.Plugins
{
    public interface IPluginLoader
    {
        bool CanLoad(string path);

        IPluginManager Load(string path);
    }
}
