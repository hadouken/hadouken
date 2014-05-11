using System.IO;
using System.Text;
using Hadouken.Fx.JsonRpc;

namespace Hadouken.Http.Management.Modules
{
    public class JsonRpcModule : ModuleBase
    {
        public JsonRpcModule(IClientTransport clientTransport)
        {
            Post["/jsonrpc"] = _ =>
            {
                using (var ms = new MemoryStream())
                {
                    Request.Body.CopyTo(ms);
                    var json = Encoding.UTF8.GetString(ms.ToArray());
                    return clientTransport.Call(json);
                }
            };
        }
    }
}
