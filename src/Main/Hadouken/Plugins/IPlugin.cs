using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hadouken.Plugins
{
    public interface IPlugin
    {
        void Load();
        void Unload();
    }
}
