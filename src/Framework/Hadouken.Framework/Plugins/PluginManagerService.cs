using System.Threading.Tasks;
using Hadouken.Framework.Rpc;

namespace Hadouken.Framework.Plugins
{
    public class PluginManagerService : IPluginManagerService
    {
        private readonly IJsonRpcHandler _jsonRpcHandler;

        public PluginManagerService(IJsonRpcHandler jsonRpcHandler)
        {
            _jsonRpcHandler = jsonRpcHandler;
        }

        public async Task<string> RpcAsync(string request)
        {
            return await _jsonRpcHandler.HandleAsync(request);
        }

        public async Task<byte[]> GetFileAsync(string path)
        {
            throw new System.NotImplementedException();
        }
    }
}