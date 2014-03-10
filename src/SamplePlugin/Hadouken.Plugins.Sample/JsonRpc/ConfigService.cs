using System.IO;
using Hadouken.Fx.JsonRpc;

namespace Hadouken.Plugins.Sample.JsonRpc
{
    public class ConfigService : IJsonRpcService
    {
        [JsonRpcMethod("sample.config.get")]
        public object GetConfig()
        {
            return new[] {1, 2, 3, 4, 5};
        }

        [JsonRpcMethod("sample.config.set")]
        public bool SetConfig(int[] values)
        {
            return true;
        }

        [JsonRpcMethod("sample.config.template")]
        public byte[] GetTemplate()
        {
            return GetResource("config.html");
        }

        [JsonRpcMethod("sample.config.script")]
        public byte[] GetScript()
        {
            return GetResource("js.config.js");
        }

        private byte[] GetResource(string name)
        {
            var resourceName = "Hadouken.Plugins.Sample.UI." + name;
            var assembly = typeof(ConfigService).Assembly;

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
