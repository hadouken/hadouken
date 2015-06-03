using System;

namespace Hadouken.Core.JsonRpc {
    [Serializable]
    public class InvalidRequestException : Exception {
        public InvalidRequestException(string message)
            : base(message) {}
    }
}