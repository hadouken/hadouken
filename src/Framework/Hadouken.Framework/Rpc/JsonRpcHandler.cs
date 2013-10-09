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
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly IRequestHandler _requestHandler;

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
            JsonRpcRequest request;
            Exception requestParseException;

            if (!JsonRpcRequest.TryParse(jsonRpc, out request, out requestParseException))
            {
                Logger.ErrorException("Could not parse request", requestParseException);
                return JsonRpcErrorResponse.ParseError(null).Serialize();
            }

            var result = _requestHandler.Execute(request);
            return result.Serialize();
        }
    }
}
