using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Hadouken.Configuration;
using NuGet;
using ILogger = Serilog.ILogger;

namespace Hadouken.Startup
{
    public class PluginBootstrapperTask : IStartupTask
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private readonly IPackageRepository _packageRepository;
        private readonly IPackageManager _packageManager;

        public PluginBootstrapperTask(ILogger logger,
            IConfiguration configuration,
            IPackageRepository packageRepository,
            IPackageManager packageManager)
        {
            _logger = logger;
            _configuration = configuration;
            _packageRepository = packageRepository;
            _packageManager = packageManager;
        }

        public void Execute(string[] args)
        {
            if (args != null && args.Contains("--no-http-bootstrap"))
            {
                _logger.Warning("HTTP bootstrapping disabled. No plugins will be downloaded.");
                return;
            }

            if (_configuration.HasDownloadedCorePluginPackages
                || _configuration.CorePluginPackages == null
                || !_configuration.CorePluginPackages.Any())
            {
                _logger.Information("Core packages already installed, or no packages specified.");
                return;
            }

            var packages = _configuration.CorePluginPackages.ToList();
            _logger.Information("Installing core packages {Packages}.", packages);

            foreach (var packageId in packages)
            {
                // Find latest version of packageId
                var package = _packageRepository.FindPackage(packageId);

                if (package == null)
                {
                    continue;
                }

                _packageManager.InstallPackage(package, false, false);
            }

            _configuration.HasDownloadedCorePluginPackages = true;
            _configuration.Save();
        }
    }
}