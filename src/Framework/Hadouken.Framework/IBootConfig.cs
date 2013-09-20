using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hadouken.Framework
{
    public interface IBootConfig
    {
        int Port { get; }

        string HostBinding { get; }

        string ApiBaseUri { get; }
    }
}
