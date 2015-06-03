using System.Linq;
using System.Reflection;

namespace Hadouken.Core.JsonRpc {
    public class Method : IMethod {
        private readonly MethodInfo _methodInfo;
        private readonly object _target;

        public Method(object target, MethodInfo methodInfo) {
            this._target = target;
            this._methodInfo = methodInfo;
        }

        public IParameter[] Parameters {
            get { return this._methodInfo.GetParameters().Select(p => new Parameter(p) as IParameter).ToArray(); }
        }

        public object Execute(object[] parameters) {
            return this._methodInfo.Invoke(this._target, parameters);
        }
    }
}