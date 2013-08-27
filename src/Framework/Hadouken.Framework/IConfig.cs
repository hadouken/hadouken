using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hadouken.Framework
{
    public interface IConfig
    {
        int Port { get; }

        string HostBinding { get; }
    }
}
