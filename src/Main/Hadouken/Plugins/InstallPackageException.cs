using System;

namespace Hadouken.Plugins
{
    public class InstallPackageException : Exception
    {
        public InstallPackageException(string message)
            : base(message)
        {
        }
    }
}