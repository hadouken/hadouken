using System;
using System.Net;

namespace Hadouken.Framework
{
    [Serializable]
    public sealed class BootConfig : IBootConfig
    {
        public string DataPath { get; set; }

        public int Port { get; set; }

        public string HostBinding { get; set; }

        public NetworkCredential Credentials { get; set; }
    }
}
