using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Hadouken.Framework.Rpc
{
    public class RequestHandler : IRequestHandler
    {
        private readonly IDictionary<string, IRpcMethod> _rpcMethods = new Dictionary<string, IRpcMethod>();

        public RequestHandler(IEnumerable<IRpcMethod> methods)
        {
            BuildMethodCache(methods);
        }

        private void BuildMethodCache(IEnumerable<IRpcMethod> methods)
        {
            foreach (var method in methods)
            {
                var methodType = method.GetType();
                var attribute = methodType.GetCustomAttribute<RpcMethodAttribute>();

                if (attribute == null)
                    continue;

                _rpcMethods.Add(attribute.Name, method);
            }
        }

        public IResponse Execute(IRequest request)
        {
            if (request == null)
                return new ErrorResponse(new ParseError());

            var method = GetRpcMethod(request.Method);

            if (method == null)
            {
                return new ErrorResponse(new MethodDoesNotExistError());
            }

            var reflectedExecuteMethod = method.GetType()
                .GetMethod("Execute", BindingFlags.Public | BindingFlags.Instance);

            var parameterType = reflectedExecuteMethod.GetParameters().First().ParameterType;
            var parameter = request.GetParameterObject(parameterType);

            var result = reflectedExecuteMethod.Invoke(method, new[] {parameter});

            return new SuccessResponse(result);
        }

        private IRpcMethod GetRpcMethod(string methodName)
        {
            lock (_rpcMethods)
            {
                IRpcMethod method;

                if (_rpcMethods.TryGetValue(methodName, out method))
                    return method;

                return null;
            }
        }
    }
}
