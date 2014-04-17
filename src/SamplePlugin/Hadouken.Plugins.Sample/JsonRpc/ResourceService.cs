using System.IO;
using Hadouken.Fx.JsonRpc;

namespace Hadouken.Plugins.Sample.JsonRpc
{
    public class ResourceService : IJsonRpcService
    {
        [JsonRpcMethod("sample.resource.get")]
        public byte[] GetResource(string name)
        {
            var resourceName = "Hadouken.Plugins.Sample.UI." + name.Replace("/", ".");
            var assembly = typeof(ResourceService).Assembly;

            using (var resource = assembly.GetManifestResourceStream(resourceName))
            using (var ms = new MemoryStream())
            {
                if (resource == null)
                {
                    return null;
                }

                resource.CopyTo(ms);
                return ms.ToArray();
            }
        }
    }
}
