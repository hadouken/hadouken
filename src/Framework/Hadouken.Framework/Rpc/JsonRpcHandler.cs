using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Hadouken.Framework.Rpc
{
    public class JsonRpcHandler : IJsonRpcHandler
    {
        private readonly IDictionary<string, IMethodInvoker> _services;

        public JsonRpcHandler(IEnumerable<IJsonRpcService> services)
        {
            _services = BuildServiceCache(services);
        }

        public Task<string> HandleAsync(string jsonRpc, object state = null)
        {
            return Task.Run(() => Execute(jsonRpc));
        }

        private IDictionary<string, IMethodInvoker> BuildServiceCache(IEnumerable<IJsonRpcService> services)
        {
            var result = new Dictionary<string, IMethodInvoker>();

            foreach (var service in services)
            {
                var type = service.GetType();
                var methods = type.GetMethods();

                foreach (var method in methods)
                {
                    var attribute = method.GetCustomAttribute<JsonRpcMethodAttribute>();

                    if (attribute == null)
                        continue;

                    var methodName = attribute.MethodName;

                    result.Add(methodName, new MethodInvoker(service, method));
                }
            }

            return result;
        }

        private string Execute(string jsonRpc)
        {
            JsonRpcRequest request;
            Exception requestParseException;

            if (!JsonRpcRequest.TryParse(jsonRpc, out request, out requestParseException))
            {
                throw new Exception();
            }

            IMethodInvoker invoker;

            if (!_services.TryGetValue(request.Method, out invoker))
            {
                throw new Exception();
            }

            var param = request.Parameters as JContainer;

            if (param != null)
            {
                if (param.Count != invoker.ParameterTypes.Length)
                {
                    throw new Exception();
                }

                var p = new List<object>();

                switch (param.Type)
                {
                    case JTokenType.Array:
                        p.AddRange(param.Select(
                            (t, i) =>
                                t.ToObject(invoker.ParameterTypes[i]))
                            .ToArray());
                        break;

                    case JTokenType.Object:
                        p.Add(param.ToObject(invoker.ParameterTypes[0]));
                        break;

                    default:
                        throw new NotSupportedException();
                }

                var result = new
                {
                    id = request.Id,
                    jsonrpc = "2.0",
                    result = invoker.Invoke(p.ToArray())
                };

                return JsonConvert.SerializeObject(result);
            }
            else
            {
                if (invoker.ParameterTypes.Length > 0
                    && request.Parameters.GetType() != invoker.ParameterTypes[0])
                {
                    throw new Exception();
                }

                var result = new
                {
                    id = request.Id,
                    jsonrpc = "2.0",
                    result = request.Parameters == null ? invoker.Invoke() : invoker.Invoke(request.Parameters)
                };

                return JsonConvert.SerializeObject(result);
            }
        }
    }
}
