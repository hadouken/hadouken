using System;

namespace Hadouken.Fx
{
    [Serializable]
    public sealed class PluginConfiguration
    {
        public string DataPath { get; set; }

        public string HttpHostBinding { get; set; }

        public int HttpPort { get; set; }

        public string HttpUserName { get; set; }

        public string HttpPasswordHash { get; set; }

        public string PluginName { get; set; }
    }
}
