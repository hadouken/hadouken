using System;

namespace Hadouken.Fx.JsonRpc
{
    [AttributeUsage(AttributeTargets.Method)]
    public class JsonRpcMethodAttribute : Attribute
    {
        private readonly string _methodName;

        public JsonRpcMethodAttribute(string methodName)
        {
            _methodName = methodName;
        }

        public string MethodName
        {
            get { return _methodName; }
        }
    }
}
