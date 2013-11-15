using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Hadouken.Framework.Plugins;
using Hadouken.Framework.Rpc;
using Hadouken.Plugins;

namespace Hadouken.Gateway
{
    public class GatewayPluginManagerService : IPluginManagerService
    {
        private readonly IJsonRpcHandler _rpcHandler;

        public GatewayPluginManagerService(IJsonRpcHandler rpcHandler)
        {
            _rpcHandler = rpcHandler;
        }

        public async Task<string> RpcAsync(string request)
        {
            return await _rpcHandler.HandleAsync(request);
        }
    }
}
