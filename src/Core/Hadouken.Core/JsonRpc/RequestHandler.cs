using System;
using System.Collections.Generic;
using System.Linq;
using Hadouken.Common.JsonRpc;

namespace Hadouken.Core.JsonRpc
{
    public class RequestHandler : IRequestHandler
    {
        private readonly IEnumerable<IParameterResolver> _parameterResolvers;
        private readonly IMethodCache _methodCache;

        public RequestHandler(IMethodCacheBuilder cacheBuilder, IEnumerable<IParameterResolver> parameterResolvers)
        {
            if (cacheBuilder == null) throw new ArgumentNullException("cacheBuilder");
            if (parameterResolvers == null) throw new ArgumentNullException("parameterResolvers");

            _parameterResolvers = parameterResolvers;
            _methodCache = cacheBuilder.Build();
        }

        public JsonRpcResponse Handle(JsonRpcRequest request)
        {
            try
            {
                // Get method
                var method = _methodCache.Get(request.MethodName);
                if (method == null)
                {
                    return new JsonRpcErrorResponse(request.Id, -32601, "Method not found");
                }

                // Find resolver
                var resolver = _parameterResolvers.FirstOrDefault(r => r.CanResolve(request.Parameters));
                if (resolver == null)
                {
                    return new JsonRpcErrorResponse(request.Id, -32602, "Invalid params");
                }

                // Resolve parameters
                var parameters = resolver.Resolve(request.Parameters, method.Parameters);

                // Execute method with resolved parameters
                var result = method.Execute(parameters);

                // Return response
                return new JsonRpcSuccessResponse(request.Id, result);
            }
            catch (Exception exception)
            {
                if (exception.InnerException is JsonRpcException)
                {
                    var inner = exception.InnerException as JsonRpcException;

                    var message = "Internal error";
                    if (!string.IsNullOrEmpty(inner.Message)) message = inner.Message;

                    return new JsonRpcErrorResponse(request.Id, inner.ErrorCode, message, inner.ToString());
                }

                return new JsonRpcErrorResponse(request.Id, -32603, "Internal error", exception.ToString());
            }
        }
    }
}