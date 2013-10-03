using System;

namespace Hadouken.Plugins
{
    [Serializable]
    public class DependencyNotFoundException : Exception
    {
        public DependencyNotFoundException() {}

        public DependencyNotFoundException(string message) : base(message) {}
    }
}
