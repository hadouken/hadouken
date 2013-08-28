using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hadouken.Framework.Events;
using Hadouken.Framework.Plugins;

namespace Hadouken.Framework
{
    public abstract class Bootstrapper
    {
        protected Bootstrapper()
        {
        }

        public abstract Plugin Load(IBootConfig config);
    }
}
