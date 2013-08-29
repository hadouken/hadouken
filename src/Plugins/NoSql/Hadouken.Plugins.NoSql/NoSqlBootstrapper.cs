using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hadouken.Framework;
using Hadouken.Framework.Plugins;

namespace Hadouken.Plugins.NoSql
{
    public class NoSqlBootstrapper : Bootstrapper
    {
        public override Plugin Load(IBootConfig config)
        {
            return new NoSqlPlugin();
        }
    }
}
