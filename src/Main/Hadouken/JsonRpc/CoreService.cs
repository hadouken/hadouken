using System.Reflection;
using Hadouken.Fx.JsonRpc;

namespace Hadouken.JsonRpc
{
    public class CoreService : IJsonRpcService
    {
        [JsonRpcMethod("core.getVersion")]
        public string GetVersion()
        {
            var attribute = typeof (CoreService).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>();
            return attribute == null ? "0.0.0" : attribute.InformationalVersion;
        }
    }
}
