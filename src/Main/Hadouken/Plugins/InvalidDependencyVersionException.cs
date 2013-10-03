using System;

namespace Hadouken.Plugins
{
    [Serializable]
    public class InvalidDependencyVersionException : Exception
    {
        public InvalidDependencyVersionException() {}

        public InvalidDependencyVersionException(string message) : base(message) {}
    }
}
