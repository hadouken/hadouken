using System.IO;

namespace Hadouken.Plugins
{
    public class PackageFactory : IPackageFactory
    {
        public IPackage ReadFrom(Stream stream)
        {
            IPackage package;
            if (!Package.TryParse(stream, out package))
            {
                return null;
            }
            return package;
        }

        public void Save(IPackage package)
        {
            //
        }
    }
}