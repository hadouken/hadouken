using System.Collections.Generic;

namespace Hadouken.Core.JsonRpc {
    public class MethodCache : IMethodCache {
        private readonly IDictionary<string, IMethod> _methods;

        public MethodCache(IDictionary<string, IMethod> methods) {
            this._methods = methods;
        }

        public IEnumerable<IMethod> GetAll() {
            return this._methods.Values;
        }

        public IMethod Get(string methodName) {
            if (this._methods.ContainsKey(methodName)) {
                return this._methods[methodName];
            }

            return null;
        }
    }
}