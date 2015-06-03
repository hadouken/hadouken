using System.IO;
using System.Text;
using Hadouken.Common.Text;
using Hadouken.Core.JsonRpc;
using Nancy;
using Nancy.Security;

namespace Hadouken.Core.Http.Modules {
    public sealed class JsonRpcModule : NancyModule {
        public JsonRpcModule(IJsonSerializer jsonSerializer,
            IJsonRpcRequestParser requestParser,
            IRequestHandler requestHandler) {
            this.Post["/jsonrpc"] = _ => {
                this.RequiresAuthentication();

                using (var ms = new MemoryStream()) {
                    this.Request.Body.CopyTo(ms);

                    var json = Encoding.UTF8.GetString(ms.ToArray());
                    var request = requestParser.Parse(json);
                    var response = requestHandler.Handle(request);

                    return this.Response.AsText(jsonSerializer.SerializeObject(response), "application/json");
                }
            };
        }
    }
}