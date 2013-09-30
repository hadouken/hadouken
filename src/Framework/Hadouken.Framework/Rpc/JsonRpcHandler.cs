using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using NLog;

namespace Hadouken.Framework.Rpc
{
    public class JsonRpcHandler : IJsonRpcHandler
    {
        private readonly IRequestHandler _requestHandler;
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private static readonly JsonSerializer Serializer = new JsonSerializer();

        static JsonRpcHandler()
        {
            Serializer.ContractResolver = new CamelCasePropertyNamesContractResolver();
            Serializer.Converters.Add(new VersionConverter());
            Serializer.Converters.Add(new StringEnumConverter());
        }

        public JsonRpcHandler(IRequestHandler requestHandler)
        {
            _requestHandler = requestHandler;
        }

        public Task<string> HandleAsync(string jsonRpc, object state = null)
        {
            return Task.Run(() => Execute(jsonRpc));
        }

        private string Execute(string jsonRpc)
        {
            Logger.Debug("Executing JSONRPC request");

            JsonRpcRequest request;
            Exception requestParseException;

            if (!JsonRpcRequest.TryParse(jsonRpc, out request, out requestParseException))
            {
                Logger.ErrorException("Could not parse request", requestParseException);

                return Serialize(new JsonRpcResponse
                    {
                        Error = new InvalidRequestError()
                    });
            }

            var result = _requestHandler.Execute(request);
            return Serialize(result);
        }

        private static string Serialize(object data)
        {
            using (var ms = new MemoryStream())
            using (var writer = new StreamWriter(ms))
            {
                Serializer.Serialize(writer, data);
                writer.Flush();

                return Encoding.UTF8.GetString(ms.ToArray());
            }
        }
    }
}
