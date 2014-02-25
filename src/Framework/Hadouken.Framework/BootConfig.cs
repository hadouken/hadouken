using System;

namespace Hadouken.Framework
{
    [Serializable]
    public sealed class BootConfig : IBootConfig
    {
        public string ApplicationBasePath { get; set; }

        public string AssemblyFile { get; set; }

        public string DataPath { get; set; }

        public int Port { get; set; }

        public string HostBinding { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public string RpcPluginUri { get; set; }

        public string RpcGatewayUri { get; set; }

        public string HttpVirtualPath { get; set; }
    }
}
