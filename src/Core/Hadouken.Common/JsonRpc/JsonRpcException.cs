using System;

namespace Hadouken.Common.JsonRpc {
    [Serializable]
    public sealed class JsonRpcException : Exception {
        private readonly int _errorCode;

        public JsonRpcException(int errorCode) {
            this._errorCode = errorCode;
        }

        public JsonRpcException(int errorCode, string message)
            : base(message) {
            this._errorCode = errorCode;
        }

        public JsonRpcException(int errorCode, string message, Exception innerException)
            : base(message, innerException) {
            this._errorCode = errorCode;
        }

        public int ErrorCode {
            get { return this._errorCode; }
        }
    }
}