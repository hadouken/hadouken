using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Hadouken.Fx.JsonRpc
{
    public class Method : IMethod
    {
        private readonly object _target;
        private readonly MethodInfo _methodInfo;

        public Method(object target, MethodInfo methodInfo)
        {
            _target = target;
            _methodInfo = methodInfo;
        }

        public IParameter[] Parameters
        {
            get { return _methodInfo.GetParameters().Select(p => new Parameter(p) as IParameter).ToArray(); }
        }

        public object Execute(object[] parameters)
        {
            return _methodInfo.Invoke(_target, parameters);
        }
    }
}