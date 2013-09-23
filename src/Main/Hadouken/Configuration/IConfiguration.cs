using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hadouken.Configuration
{
    public interface IConfiguration
    {
        string ApplicationDataPath { get; set; }

        string InstanceName { get; set; }

        PluginsCollection Plugins { get; }

        HttpConfiguration Http { get; }

        void Save();
    }
}
