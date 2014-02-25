using System;

namespace Hadouken.Fx.JsonRpc
{
    public class RequestHandler : IRequestHandler
    {
        private readonly IParameterResolver _parameterResolver;
        private readonly IMethodCache _methodCache;

        public RequestHandler(IMethodCacheBuilder builder, IParameterResolver parameterResolver)
        {
            _parameterResolver = parameterResolver;
            _methodCache = builder.Build();
        }

        public JsonRpcResponse Handle(JsonRpcRequest request)
        {
            try
            {
                // Get method
                var method = _methodCache.Get(request.MethodName);
                if (method == null)
                {
                    return new JsonRpcErrorResponse(request.Id);
                }

                // Resolve parameters
                var parameters = _parameterResolver.Resolve(request.Parameters, method.Parameters);

                // Execute method with resolved parameters
                var result = method.Execute(parameters);

                // Return response
                return new JsonRpcSuccessResponse(request.Id, result);
            }
            catch (Exception)
            {
                return new JsonRpcErrorResponse(request.Id);
            }
        }
    }
}