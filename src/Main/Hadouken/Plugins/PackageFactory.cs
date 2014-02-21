using System;
using System.Collections.Generic;
using System.Linq;
using Hadouken.Configuration;
using Hadouken.Framework.IO;

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

        public IEnumerable<IPackage> Scan()
        {
            var result = new List<IPackage>();

            result.AddRange(ScanBaseDirectory());
            result.AddRange(LoadFromExplicitList());

            var cmdLinePackage = LoadFromCommandLineArgument();

            if (cmdLinePackage != null)
            {
                result.Add(cmdLinePackage);
            }

            return result;
        }

        private IEnumerable<IPackage> ScanBaseDirectory()
        {
            var pluginDirectory = _fileSystem.GetDirectory(_configuration.Plugins.BaseDirectory);
            var result = new List<IPackage>();

            foreach (var file in pluginDirectory.Files)
            {
                IPackage package;

                if (!Package.TryParse(file, out package))
                {
                    continue;
                }

                result.Add(package);
            }

            return result;
        }

        private IEnumerable<IPackage> LoadFromExplicitList()
        {
            var result = new List<IPackage>();

            foreach (var plugin in _configuration.Plugins)
            {
                var directory = _fileSystem.GetDirectory(plugin.Path);

                IPackage package;

                if (directory.Exists)
                {
                    if (!Package.TryParse(directory, out package))
                    {
                        continue;
                    }
                }
                else
                {
                    var file = _fileSystem.GetFile(plugin.Path);

                    if (!Package.TryParse(file, out package))
                    {
                        continue;
                    }
                }

                result.Add(package);
            }

            return result;
        }

        private IPackage LoadFromCommandLineArgument()
        {
            var args = Environment.GetCommandLineArgs();
            args = args.SkipWhile(s => s != "--plugin").ToArray();

            if (!args.Any() || args.Length < 2)
            {
                return null;
            }

            if (args.First() != "--plugin")
            {
                return null;
            }

            var path = args.Skip(1).First();
            var directory = _fileSystem.GetDirectory(path);

            if (!directory.Exists)
            {
                return null;
            }

            IPackage package;

            return !Package.TryParse(directory, out package) ? null : package;
        }
    }
}