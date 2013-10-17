using System.IO;
using System.Text;
using System.Threading.Tasks;
using Hadouken.Framework.IO;
using Hadouken.Framework.Rpc;
using Hadouken.Framework.TypeScript;
using NLog;

namespace Hadouken.Framework.Plugins
{
    public class PluginManagerService : IPluginManagerService
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly IJsonRpcHandler _jsonRpcHandler;
        private readonly IRootPathProvider _rootPathProvider;
        private readonly ITypeScriptCompiler _typeScriptCompiler;

        public PluginManagerService(IJsonRpcHandler jsonRpcHandler, IRootPathProvider rootPathProvider, ITypeScriptCompiler typeScriptCompiler)
        {
            _jsonRpcHandler = jsonRpcHandler;
            _rootPathProvider = rootPathProvider;
            _typeScriptCompiler = typeScriptCompiler;
        }

        public async Task<string> RpcAsync(string request)
        {
            return await _jsonRpcHandler.HandleAsync(request);
        }

        public async Task<byte[]> GetFileAsync(string path)
        {
            var root = _rootPathProvider.GetRootPath();
            var file = Path.Combine(root, "UI", path);
            var typeScript = path.Substring(0, path.Length - 3) + ".ts";
            typeScript = Path.Combine(root, "UI", typeScript);

            if (File.Exists(typeScript))
                return Encoding.UTF8.GetBytes(_typeScriptCompiler.Compile(typeScript));

            return !File.Exists(file) ? null : File.ReadAllBytes(file);
        }
    }
}