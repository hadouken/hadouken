using System.IO;
using System.Text;
using Hadouken.Common.Text;
using Hadouken.Core.JsonRpc;
using Nancy;

namespace Hadouken.Core.Http.Modules
{
    public sealed class ApiModule : NancyModule
    {
        public ApiModule(IJsonSerializer jsonSerializer,
            IJsonRpcRequestParser requestParser,
            IRequestHandler requestHandler)
        {
            Post["/api/jsonrpc"] = _ =>
            {
                using (var ms = new MemoryStream())
                {
                    Request.Body.CopyTo(ms);

                    var json = Encoding.UTF8.GetString(ms.ToArray());
                    var request = requestParser.Parse(json);
                    var response = requestHandler.Handle(request);

                    return Response.AsText(jsonSerializer.SerializeObject(response), "application/json");
                }
            };
        }
    }
}
