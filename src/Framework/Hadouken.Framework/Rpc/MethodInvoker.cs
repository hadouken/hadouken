using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using NLog;

namespace Hadouken.Framework.Rpc
{
    public class MethodInvoker : IMethodInvoker
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly object _target;
        private readonly MethodInfo _method;

        public MethodInvoker(object target, MethodInfo method)
        {
            if (target == null)
                throw new ArgumentNullException("target");

            if (method == null)
                throw new ArgumentNullException("method");

            if (!target.GetType().GetMethods().Contains(method))
                throw new ArgumentException("target does not contain the specified method");

            _target = target;
            _method = method;
        }

        public Type[] ParameterTypes
        {
            get { return _method.GetParameters().Select(p => p.ParameterType).ToArray(); }
        }

        public object Invoke(params object[] args)
        {
            Logger.Debug("Invoking method with {0} arguments", args.Length);

            if (args.Length != ParameterTypes.Length)
                throw new ArgumentException("Mismatch in argument and parameter length");

            return _method.Invoke(_target, args);
        }
    }
}
