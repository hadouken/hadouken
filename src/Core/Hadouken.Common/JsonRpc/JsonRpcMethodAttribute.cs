using System;

namespace Hadouken.Common.JsonRpc {
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class JsonRpcMethodAttribute : Attribute {
        private readonly string _methodName;

        public JsonRpcMethodAttribute(string methodName) {
            if (methodName == null) {
                throw new ArgumentNullException("methodName");
            }
            this._methodName = methodName;
        }

        public string MethodName {
            get { return this._methodName; }
        }
    }
}