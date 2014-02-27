namespace Hadouken.Fx.JsonRpc.Services
{
    public class ResourceService : IJsonRpcService
    {
        [JsonRpcMethod("resource.get")]
        public byte[] GetResource(string name)
        {
            return new byte[] {1, 2, 3};
        }
    }
}
