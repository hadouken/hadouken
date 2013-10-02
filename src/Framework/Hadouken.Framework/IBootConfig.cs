using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hadouken.Framework
{
    public interface IBootConfig
    {
        string DataPath { get; }

        int Port { get; }

        string HostBinding { get; }
    }
}
