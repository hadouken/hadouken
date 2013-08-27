using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hadouken.Framework.Plugins
{
    public abstract class Plugin
    {
        public abstract void Load();

        public virtual void Unload() {}
    }
}
