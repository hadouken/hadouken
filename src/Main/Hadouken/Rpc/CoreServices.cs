using System;
using System.Collections.Generic;
using Hadouken.Configuration;
using Hadouken.Framework.Rpc;
using Newtonsoft.Json.Linq;

namespace Hadouken.Rpc
{
    public class CoreServices : IJsonRpcService
    {
        private readonly IConfiguration _configuration;
        private readonly JsonRpcClient _rpcClient;

        public CoreServices(IConfiguration configuration)
        {
            _configuration = configuration;
            _rpcClient =
                new JsonRpcClient(
                    new Uri(String.Format("http://{0}:{1}/jsonrpc", configuration.Http.HostBinding,
                        configuration.Http.Port)));
        }

        [JsonRpcMethod("core.multiCall")]
        public object MultiCall(Dictionary<string, object> call)
        {
            var result = new Dictionary<string, object>();

            foreach (var key in call.Keys)
            {
                var callResult = _rpcClient.CallAsync<object>(key, call[key]).Result;
                result.Add(key, callResult);
            }

            return result;
        }
    }
}
