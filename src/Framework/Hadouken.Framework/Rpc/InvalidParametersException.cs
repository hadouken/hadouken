using System;
namespace Hadouken.Framework.Rpc
{
    [Serializable]
    public class InvalidParametersException : Exception
    {
        public InvalidParametersException() {}

        public InvalidParametersException(string message) : base(message) {}

        public InvalidParametersException(string message, Exception innerException) : base(message, innerException) {}
    }
}
