using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;

namespace Hadouken.Framework.Rpc
{
    public sealed class JsonRpcClient : IJsonRpcClient
    {
        private readonly IClientTransport _transport;
        private static readonly JsonSerializerSettings SerializerSettings = new JsonSerializerSettings();
        private int _requestId = 0;

        static JsonRpcClient()
        {
            SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            SerializerSettings.Converters.Add(new SemanticVersionConverter());
            SerializerSettings.Converters.Add(new StringEnumConverter());
            SerializerSettings.Converters.Add(new VersionConverter());
        }

        public JsonRpcClient(IClientTransport transport)
        {
            _transport = transport;
        }

        public TResult Call<TResult>(string method, object parameters = null)
        {
            _requestId++;

            var request = new
                {
                    id = _requestId,
                    jsonrpc = "2.0",
                    method,
                    @params = parameters
                };

            var json = JsonConvert.SerializeObject(request, SerializerSettings);
            var response = _transport.Send(json);

            JsonRpcResponse rpcResponse;
            Exception exception;

            if (JsonRpcResponse.TryParse(response, out rpcResponse, out exception))
            {
                var successResponse = rpcResponse as JsonRpcSuccessResponse;

                if (successResponse != null && successResponse.Result != null)
                {
                    return successResponse.Result.ToObject<TResult>();
                }
            }

            return default(TResult);
        }

        public void Dispose()
        {
        }
    }
}
