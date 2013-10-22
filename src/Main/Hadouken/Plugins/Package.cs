using System;
using System.IO;

namespace Hadouken.Plugins
{
    public abstract class Package
    {
        public static bool TryParse(Stream stream, out IPackage package)
        {
            throw new NotImplementedException();
        }
    }
}
