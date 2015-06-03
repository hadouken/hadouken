using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Hadouken.Common.JsonRpc;

namespace Hadouken.Core.JsonRpc {
    public class MethodCacheBuilder : IMethodCacheBuilder {
        private readonly IEnumerable<IJsonRpcService> _services;

        public MethodCacheBuilder(IEnumerable<IJsonRpcService> services) {
            this._services = services ?? Enumerable.Empty<IJsonRpcService>();
        }

        public IMethodCache Build() {
            var result = new Dictionary<string, IMethod>();

            foreach (var service in this._services) {
                var serviceMethods = FindRpcDecoratedMethods(service);

                foreach (var method in serviceMethods) {
                    var attribute = method.GetCustomAttribute<JsonRpcMethodAttribute>();

                    if (result.ContainsKey(attribute.MethodName)) {
                        throw new MethodNameAlreadyRegisteredException(attribute.MethodName);
                    }

                    result.Add(attribute.MethodName, new Method(service, method));
                }
            }

            return new MethodCache(result);
        }

        private static IEnumerable<MethodInfo> FindRpcDecoratedMethods(object target) {
            var methods = target.GetType().GetMethods();
            return (from method in methods
                let attr = method.GetCustomAttribute<JsonRpcMethodAttribute>()
                where attr != null
                select method).ToArray();
        }
    }
}