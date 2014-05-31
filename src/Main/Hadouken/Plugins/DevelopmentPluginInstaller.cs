using System;
using System.IO;
using System.Linq;
using NuGet;
using IFileSystem = Hadouken.Fx.IO.IFileSystem;
using ILogger = Serilog.ILogger;

namespace Hadouken.Plugins
{
    public class DevelopmentPluginInstaller : IDevelopmentPluginInstaller
    {
        private readonly ILogger _logger;
        private readonly IFileSystem _fileSystem;

        public DevelopmentPluginInstaller(ILogger logger, IFileSystem fileSystem)
        {
            _logger = logger;
            _fileSystem = fileSystem;
        }

        public IPackage GetPackage()
        {
            var args = Environment.GetCommandLineArgs();
            var spec = GetArgValue(args, "--plugin-nuspec");
            var path = GetArgValue(args, "--plugin-path");

            if (string.IsNullOrEmpty(spec) || string.IsNullOrEmpty(path))
            {
                _logger.Debug("No development package found.");
                return null;
            }

            var builder = new PackageBuilder(spec, path, new NullProvider(), false);
            var outputPath = Path.GetTempFileName();

            _logger.Information("Building development package at {Path}", outputPath);

            using (var stream = File.Create(outputPath))
            {
                builder.Save(stream);
            }

            return new OptimizedZipPackage(outputPath);
        }

        private static string GetArgValue(string[] args, string argName)
        {
            if (!args.Any() || args.Length < 2)
            {
                return string.Empty;
            }

            var remainder = args.SkipWhile(a => a != argName).ToArray();

            if (!remainder.Any()
                || remainder.Length < 2
                || remainder.First() != argName)
            {
                return string.Empty;
            }

            return remainder.Skip(1).First();
        }
    }

    public class NullProvider : IPropertyProvider
    {
        public dynamic GetPropertyValue(string propertyName)
        {
            return null;
        }
    }
}
