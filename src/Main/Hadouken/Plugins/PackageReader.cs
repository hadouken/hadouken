using System.IO;

namespace Hadouken.Plugins
{
    public class PackageReader : IPackageReader
    {
        public IPackage Read(Stream stream)
        {
            IPackage package;
            if (!Package.TryParse(stream, out package))
            {
                return null;
            }
            return package;
        }
    }
}