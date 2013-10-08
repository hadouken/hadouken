using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NLog;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace Hadouken.Framework.Rpc
{
    public class RequestHandler : IRequestHandler
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private static readonly JsonSerializer Serializer = new JsonSerializer();
        private readonly IDictionary<string, IMethodInvoker> _services;

        static RequestHandler()
        {
            Serializer.ContractResolver = new CamelCasePropertyNamesContractResolver();
            Serializer.Converters.Add(new VersionConverter());
            Serializer.Converters.Add(new StringEnumConverter());
        }

        public RequestHandler(IEnumerable<IJsonRpcService> services)
        {
            _services = BuildServiceCache(services);            
        }

        private IDictionary<string, IMethodInvoker> BuildServiceCache(IEnumerable<IJsonRpcService> services)
        {
            Logger.Info("Building service cache");

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

                    Logger.Debug("Adding MethodInvoker for {0}", methodName);
                    result.Add(methodName, new MethodInvoker(service, method));
                }
            }

            return result;
        }

        protected virtual JsonRpcResponse OnMethodMissing(JsonRpcRequest request)
        {
            Logger.Info("Could not find method {0} in cache.", request.Method);

            return new JsonRpcResponse
                {
                    Error = new MethodNotFoundError
                        {
                            Data = String.Format("Method {0} not found.", request.Method)
                        }
                };
        }

        public JsonRpcResponse Execute(JsonRpcRequest request)
        {
            IMethodInvoker invoker;

            if (!_services.TryGetValue(request.Method, out invoker))
            {
                return OnMethodMissing(request);
            }

            var param = request.Parameters as JContainer;

            if (param != null)
            {
                var p = new List<object>();

                switch (param.Type)
                {
                    case JTokenType.Array:
                        if (param.Count != invoker.ParameterTypes.Length)
                        {
                            return new JsonRpcResponse
                            {
                                Id = request.Id,
                                Error = new InvalidParamsError()
                            };
                        }

                        p.AddRange(param.Select(
                            (t, i) =>
                                t.ToObject(invoker.ParameterTypes[i], Serializer))
                            .ToArray());
                        break;

                    case JTokenType.Object:
                        p.Add(param.ToObject(invoker.ParameterTypes[0], Serializer));
                        break;

                    default:
                        return new JsonRpcResponse
                            {
                                Id = request.Id,
                                Error = new InternalRpcError()
                            };
                }

                return new JsonRpcResponse
                    {
                        Id = request.Id,
                        Result = invoker.Invoke(p.ToArray())
                    };
            }

            if (invoker.ParameterTypes.Length > 0
                && request.Parameters.GetType() != invoker.ParameterTypes[0])
            {
                return new JsonRpcResponse
                    {
                        Id = request.Id,
                        Error = new InvalidParamsError()
                    };
            }

            return new JsonRpcResponse
                {
                    Id = request.Id,
                    Result = request.Parameters == null ? invoker.Invoke() : invoker.Invoke(request.Parameters)
                };
        }
    }
}