using System;

namespace Hadouken.Core.JsonRpc {
    public class MethodNameAlreadyRegisteredException : Exception {
        private readonly string _methodName;

        public MethodNameAlreadyRegisteredException(string methodName) {
            this._methodName = methodName;
        }

        public string MethodName {
            get { return this._methodName; }
        }
    }
}