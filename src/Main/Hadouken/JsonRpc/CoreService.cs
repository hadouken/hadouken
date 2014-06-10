using Hadouken.Fx.JsonRpc;

namespace Hadouken.JsonRpc
{
    public class CoreService : IJsonRpcService
    {
        [JsonRpcMethod("core.getInfo")]
        public object GetInformation()
        {
            return new
            {
                AssemblyInformation.BranchName,
                AssemblyInformation.BuildDate,
                AssemblyInformation.Commit
            };
        }
    }
}
