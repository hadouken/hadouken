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

        public ParameterInfo[] Parameters
        {
            get { return _method.GetParameters().ToArray(); }
        }

        public object Invoke(params object[] args)
        {
            var argsLength = (args == null ? 0 : args.Length);
            Logger.Debug("Invoking method {0} with {1} arguments", _target.GetType().Name + "." + _method.Name, argsLength);

            if (argsLength != Parameters.Length)
                throw new ArgumentException("Mismatch in argument and parameter length");

            return _method.Invoke(_target, args);
        }
    }
}
