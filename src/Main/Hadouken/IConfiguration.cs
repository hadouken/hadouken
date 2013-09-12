using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hadouken
{
    public interface IConfiguration
    {
        string PluginsPath { get; }

        string HostBinding { get; }

        int Port { get; }
    }
}
