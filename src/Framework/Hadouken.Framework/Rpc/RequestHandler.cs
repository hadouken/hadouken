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

        internal IDictionary<string, IMethodInvoker> Services
        {
            get { return _services; }
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
            return JsonRpcErrorResponse.MethodNotFound(request.Method, request.Method);
        }

        public JsonRpcResponse Execute(JsonRpcRequest request)
        {
            IMethodInvoker invoker;

            if (!_services.TryGetValue(request.Method, out invoker))
            {
                return OnMethodMissing(request);
            }

            IParameterResolver paramResolver = new ParameterResolver();

            try
            {
                var param = paramResolver.Resolve(request.Parameters, invoker);
                var result = invoker.Invoke(param);

                var token = (JToken)null;

                if (result != null)
                    token = JToken.FromObject(result, Serializer);

                return new JsonRpcSuccessResponse() {Id = request.Id, Result = token};
            }
            catch (InvalidParametersException)
            {
                return JsonRpcErrorResponse.InvalidParams(request.Id);
            }
            catch (Exception exception)
            {
                return JsonRpcErrorResponse.InternalRpcError(request.Id, exception);
            }
        }
    }
}