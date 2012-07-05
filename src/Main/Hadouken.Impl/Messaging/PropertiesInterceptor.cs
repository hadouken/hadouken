using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Castle.DynamicProxy;
using System.Reflection;

namespace Hadouken.Messaging
{
    public class PropertiesInterceptor : MarshalByRefObject, IInterceptor
    {
        Dictionary<string, object> _properties = new Dictionary<string, object>();

        public virtual void Intercept(IInvocation invocation)
        {
            if (IsGetter(invocation.Method))
            {
                invocation.ReturnValue = GetValue(invocation.Method);
            }
            else if (IsSetter(invocation.Method))
            {
                SetValue(invocation.Method, invocation.Arguments[0]);
            }
            else
            {
                throw new NotSupportedException("Properties only!");
            }
        }

        private void SetValue(MethodInfo method, object propertyValue)
        {
            var propertyName = GetName(method);
            _properties.Add(propertyName, propertyValue);
        }

        private object GetValue(MethodInfo method)
        {
            var propertyName = GetName(method);
            object propertyValue;

            if (!_properties.TryGetValue(propertyName, out propertyValue))
                propertyValue = Activator.CreateInstance(method.ReturnType);

            return propertyValue;
        }

        protected string GetName(MethodInfo method)
        {
            return method.Name.Substring(4);
        }

        protected bool IsGetter(MethodInfo method)
        {
            return
                method.IsSpecialName &&
                method.Name.Length > 4 &&
                method.Name.StartsWith("get_", StringComparison.OrdinalIgnoreCase);
        }

        protected bool IsSetter(MethodInfo method)
        {
            return
                method.IsSpecialName &&
                method.Name.Length > 4 &&
                method.Name.StartsWith("set_", StringComparison.OrdinalIgnoreCase);
        }
    }

}
