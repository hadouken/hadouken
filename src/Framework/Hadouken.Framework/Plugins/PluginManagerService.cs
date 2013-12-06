using System.IO;
using System.Threading.Tasks;
using Hadouken.Framework.IO;
using Hadouken.Framework.Rpc;
using NLog;

namespace Hadouken.Framework.Plugins
{
    public class PluginManagerService : IPluginManagerService
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly IJsonRpcHandler _jsonRpcHandler;

        public PluginManagerService(IJsonRpcHandler jsonRpcHandler)
        {
            _jsonRpcHandler = jsonRpcHandler;
        }

        public async Task<string> RpcAsync(string request)
        {
            return await _jsonRpcHandler.HandleAsync(request);
        }
    }
}