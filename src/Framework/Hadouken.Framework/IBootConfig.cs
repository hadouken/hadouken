namespace Hadouken.Framework
{
    public interface IBootConfig
    {
        string ApplicationBasePath { get; }

        string AssemblyFile { get; }

        string DataPath { get; }

        int Port { get; }

        string HttpVirtualPath { get; }

        string RpcGatewayUri { get; }

        string RpcPluginUri { get; }

        string HostBinding { get; }

        string UserName { get; }
        
        string Password { get; }
    }
}
