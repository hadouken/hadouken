using Hadouken.Common.JsonRpc;

namespace Hadouken.Core.Services
{
    public sealed class CoreService : IJsonRpcService
    {
        [JsonRpcMethod("core.getVersion")]
        public object GetVersion()
        {
            return new
            {
                AssemblyInformation.BuildDate,
                AssemblyInformation.Commit,
                Version = typeof (AssemblyInformation).Assembly.GetName().Version.ToString()
            };
        }
    }
}
