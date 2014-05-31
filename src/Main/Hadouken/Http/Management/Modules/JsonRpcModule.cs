using System.IO;
using System.Text;
using Hadouken.Fx.JsonRpc;
using Nancy;
using Nancy.Security;

namespace Hadouken.Http.Management.Modules
{
    public sealed class JsonRpcModule : NancyModule
    {
        public JsonRpcModule(IClientTransport clientTransport)
        {
            this.RequiresAuthentication();

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
