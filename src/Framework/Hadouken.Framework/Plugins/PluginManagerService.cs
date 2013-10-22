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
        private readonly IRootPathProvider _rootPathProvider;

        public PluginManagerService(IJsonRpcHandler jsonRpcHandler, IRootPathProvider rootPathProvider)
        {
            _jsonRpcHandler = jsonRpcHandler;
            _rootPathProvider = rootPathProvider;
        }

        public async Task<string> RpcAsync(string request)
        {
            return await _jsonRpcHandler.HandleAsync(request);
        }

        public async Task<byte[]> GetFileAsync(string path)
        {
            // TODO: Use some kind of abstraction

            var root = _rootPathProvider.GetRootPath();
            var file = Path.Combine(root, "UI", path);

            return File.ReadAllBytes(file);
        }
    }
}