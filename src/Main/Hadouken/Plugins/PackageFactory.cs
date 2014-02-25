using System.Collections.Generic;
using System.IO;
using Hadouken.Configuration;
using Hadouken.Fx.IO;

namespace Hadouken.Plugins
{
    public class PackageFactory : IPackageFactory
    {
        private readonly IConfiguration _configuration;
        private readonly IFileSystem _fileSystem;

        public PackageFactory(IConfiguration configuration, IFileSystem fileSystem)
        {
            _configuration = configuration;
            _fileSystem = fileSystem;
        }

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

        public IEnumerable<IPackage> Scan()
        {
            var result = new List<IPackage>();

            result.AddRange(ScanBaseDirectory());

            return result;
        }

        private IEnumerable<IPackage> ScanBaseDirectory()
        {
            var pluginDirectory = _fileSystem.GetDirectory(_configuration.Plugins.BaseDirectory);
            var result = new List<IPackage>();

            foreach (var file in pluginDirectory.Files)
            {
                using (var stream = file.OpenRead())
                {
                    IPackage package;
                    if (!Package.TryParse(stream, out package))
                    {
                        continue;
                    }

                    result.Add(package);
                }
            }

            return result;
        }
    }
}