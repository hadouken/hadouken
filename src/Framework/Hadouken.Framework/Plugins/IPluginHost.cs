using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hadouken.Framework.Plugins
{
    public interface IPluginHost
    {
        void Load();

        void Unload();
    }
}
