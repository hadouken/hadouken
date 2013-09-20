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

namespace Hadouken.Framework.Rpc
{
    public class JsonRpcHandler : IJsonRpcHandler
    {
        private static readonly JsonSerializer Serializer = new JsonSerializer();
        private readonly IDictionary<string, IMethodInvoker> _services;

        static JsonRpcHandler()
        {
            Serializer.ContractResolver = new CamelCasePropertyNamesContractResolver();
            Serializer.Converters.Add(new VersionConverter());
            Serializer.Converters.Add(new StringEnumConverter());
        }

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

        protected virtual string OnMethodMissing(string methodName, string rawRequest)
        {
            return null;
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
                var data = OnMethodMissing(request.Method, jsonRpc);
                return data;
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
                                t.ToObject(invoker.ParameterTypes[i], Serializer))
                            .ToArray());
                        break;

                    case JTokenType.Object:
                        p.Add(param.ToObject(invoker.ParameterTypes[0], Serializer));
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


                return Serialize(result);
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

                return Serialize(result);
            }
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
