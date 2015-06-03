using System;
using System.Collections.Generic;
using System.Linq;
using Hadouken.Common.JsonRpc;

namespace Hadouken.Core.JsonRpc {
    public class RequestHandler : IRequestHandler {
        private readonly IMethodCache _methodCache;
        private readonly IEnumerable<IParameterResolver> _parameterResolvers;

        public RequestHandler(IMethodCacheBuilder cacheBuilder, IEnumerable<IParameterResolver> parameterResolvers) {
            if (cacheBuilder == null) {
                throw new ArgumentNullException("cacheBuilder");
            }
            if (parameterResolvers == null) {
                throw new ArgumentNullException("parameterResolvers");
            }

            this._parameterResolvers = parameterResolvers;
            this._methodCache = cacheBuilder.Build();
        }

        public JsonRpcResponse Handle(JsonRpcRequest request) {
            try {
                // Get method
                var method = this._methodCache.Get(request.MethodName);
                if (method == null) {
                    return new JsonRpcErrorResponse(request.Id, -32601, "Method not found");
                }

                // Find resolver
                var resolver = this._parameterResolvers.FirstOrDefault(r => r.CanResolve(request.Parameters));
                if (resolver == null) {
                    return new JsonRpcErrorResponse(request.Id, -32602, "Invalid params");
                }

                // Resolve parameters
                var parameters = resolver.Resolve(request.Parameters, method.Parameters);

                // Execute method with resolved parameters
                var result = method.Execute(parameters);

                // Return response
                return new JsonRpcSuccessResponse(request.Id, result);
            }
            catch (Exception exception) {
                if (!(exception.InnerException is JsonRpcException)) {
                    return new JsonRpcErrorResponse(request.Id, -32603, "Internal error", exception.ToString());
                }
                var inner = (JsonRpcException) exception.InnerException;

                var message = "Internal error";
                if (!string.IsNullOrEmpty(inner.Message)) {
                    message = inner.Message;
                }

                return new JsonRpcErrorResponse(request.Id, inner.ErrorCode, message, inner.ToString());
            }
        }
    }
}