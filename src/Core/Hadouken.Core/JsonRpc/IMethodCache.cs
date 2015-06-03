using System.Collections.Generic;

namespace Hadouken.Core.JsonRpc {
    public interface IMethodCache {
        IEnumerable<IMethod> GetAll();
        IMethod Get(string methodName);
    }
}