using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hadouken.Framework
{
    [Serializable]
    public sealed class BootConfig : IBootConfig
    {
        public int Port { get; set; }

        public string HostBinding { get; set; }

        public string ApiBaseUri { get; set; }
    }
}
