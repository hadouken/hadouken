using Hadouken.Framework.Rpc.Hosting;

namespace Hadouken.Plugins.HttpJsonRpc
{
    public interface IHttpJsonRpcServer : IJsonRpcServer
    {
        void SetCredentials(string userName, string password);
    }
}