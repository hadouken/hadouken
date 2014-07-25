using System.Collections.Generic;

namespace Hadouken.Core.JsonRpc
{
    public class MethodCache : IMethodCache
    {
        private readonly IDictionary<string, IMethod> _methods;
 
        public MethodCache(IDictionary<string, IMethod> methods)
        {
            _methods = methods;
        }

        public IEnumerable<IMethod> GetAll()
        {
            return _methods.Values;
        }

        public IMethod Get(string methodName)
        {
            if (_methods.ContainsKey(methodName))
            {
                return _methods[methodName];
            }

            return null;
        }
    }
}