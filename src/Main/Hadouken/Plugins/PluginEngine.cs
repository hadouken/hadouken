using System;
using System.Collections.Generic;
using System.Linq;
using Serilog;

namespace Hadouken.Plugins
{
    public class PluginEngine : IPluginEngine
    {
        private readonly PluginManagerGraph _plugins = new PluginManagerGraph();

        private readonly ILogger _logger;
        private readonly IEnumerable<IPluginScanner> _pluginScanners;
        private readonly IPackageInstaller _packageInstaller;
        private readonly IPackageDownloader _packageDownloader;

        public PluginEngine(ILogger logger, IEnumerable<IPluginScanner> pluginScanners, IPackageInstaller packageInstaller, IPackageDownloader packageDownloader)
        {
            if (logger == null) throw new ArgumentNullException("logger");

            if (pluginScanners == null)
            {
                throw new ArgumentNullException("pluginScanners");
            }

            if (packageInstaller == null)
            {
                throw new ArgumentNullException("packageInstaller");
            }

            if (packageDownloader == null)
            {
                throw new ArgumentNullException("packageDownloader");
            }

            _logger = logger;
            _pluginScanners = pluginScanners;
            _packageInstaller = packageInstaller;
            _packageDownloader = packageDownloader;
        }

        public IEnumerable<IPluginManager> GetAll()
        {
            return _plugins.GetAll();
        } 

        public IPluginManager Get(string name)
        {
            return _plugins.Get(name);
        }

        public string[] GetUnloadOrder(string name)
        {
            return _plugins.GetUnloadOrder(name);
        }

        public void Scan()
        {
            // Select all plugins from all scanners.
            var plugins = (from scanner in _pluginScanners
                from plugin in scanner.Scan()
                where Get(plugin.Manifest.Name) == null
                select plugin).ToList();

            _logger.Debug("Scan found {Count} plugins to add", plugins.Count);

            // Add the new plugins
            _plugins.AddRange(plugins);
            _plugins.Rebuild();
        }

        public void LoadAll()
        {
            _logger.Debug("Loading all plugins.");

            var managerKeys = _plugins.GetKeys();

            foreach (var key in managerKeys)
            {
                Load(key);
            }
        }

        public void UnloadAll()
        {
            _logger.Debug("Unloading all plugins.");

            var managerKeys = _plugins.GetKeys();

            foreach (var key in managerKeys)
            {
                Unload(key);
            }
        }

        public void Load(string name)
        {
            var manager = Get(name);
            if (manager == null)
            {
                _logger.Debug("Plugin {Name} does not exist.", name);
                return;
            }

            // Check state
            if (manager.State == PluginState.Loaded)
            {
                _logger.Debug("Skipping load of plugin {Name} since it is already loaded.", name);
                return;
            }

            string[] missingDependencies;
            if (!CanLoad(name, out missingDependencies))
            {
                _logger.Warning("Plugin {Name} is missing dependencies {MissingDependencies}. Downloading...", name,
                    string.Join(",", missingDependencies));

                if (!DownloadAndInstall(missingDependencies)) return;

                Scan();
                Load(name);
            }
            else
            {
                Load(manager);                
            }

        }

        public bool CanLoad(string name, out string[] missingDependencies)
        {
            var dependencies = _plugins.GetLoadOrder(name);

            missingDependencies = null;
            var missingDeps = dependencies.Where(dependency => Get(dependency) == null).ToArray();

            if (missingDeps.Any())
            {
                missingDependencies = missingDeps;
                return false;
            }

            return true;
        }

        public bool CanUnload(string name, out string[] dependencies)
        {
            var deps = _plugins.GetUnloadOrder(name);

            dependencies = null;
            var existingDeps = deps.Where(dependency => Get(dependency) != null && dependency != name).ToArray();

            if (existingDeps.Any())
            {
                dependencies = existingDeps;
                return false;
            }

            return true;
        }

        public void Unload(string name)
        {
            var manager = Get(name);
            if (manager == null)
            {
                _logger.Debug("Plugin {Name} does not exist.", name);
                return;
            }

            // Check state
            if (manager.State == PluginState.Loaded || manager.State == PluginState.Error)
            {
                Unload(manager);
            }
            else
            {
                _logger.Debug("Skipping unload of plugin {Name} since it is already unloaded.", name);
            }
        }

        public bool InstallOrUpgrade(IPackage package)
        {
            // Check it we have a plugin with this name already. If we do,
            // we must have a higher version number 
            // the existing to be able to upgrade. Otherwise - out of luck.
            var existing = Get(package.Manifest.Name);
            string[] loadOrder = null;

            if (existing != null)
            {
                if (existing.Manifest.Version >= package.Manifest.Version)
                {
                    _logger.Error("Downgrades not supported ({Name} {Version}).", package.Manifest.Name, package.Manifest.Version);
                    return false;
                }

                // Set the load order to the reverse unload order
                loadOrder = _plugins.GetUnloadOrder(package.Manifest.Name).Reverse().ToArray();

                if (!Uninstall(existing))
                {
                    return false;
                }
            }

            // Install dat package!
            _logger.Information("Installing package {Name}, version {Version}", package.Manifest.Name, package.Manifest.Version);
            _packageInstaller.Install(package);

            Scan();

            // If we have no load order, just load the installed package.
            if (loadOrder == null)
            {
                Load(package.Manifest.Name);
            }
            else
            {
                // If we have a load order, load in that order
                foreach (var plugin in loadOrder)
                {
                    Load(plugin);
                }
            }

            return true;
        }

        public bool Uninstall(string name)
        {
            var manager = Get(name);
            if (manager == null)
            {
                _logger.Debug("Plugin {Name} does not exist.", name);
                return false;
            }

            return Uninstall(manager);
        }

        private bool Uninstall(IPluginManager manager)
        {
            _logger.Debug("Beginning uninstall of plugin {Name}", manager.Manifest.Name);

            // Unload the manager
            Unload(manager);

            // Get dependencies and check that all are unloaded
            var dependencies =
                _plugins.GetUnloadOrder(manager.Manifest.Name)
                    .Where(d => Get(d).State != PluginState.Unloaded)
                    .ToArray();

            if (dependencies.Any())
            {
                _logger.Error("Cannot uninstall plugin {Name}. The plugins {Dependencies} could not be unloaded.", manager.Manifest.Name,
                    string.Join(",", dependencies));
                return false;
            }

            _logger.Information("Uninstalling plugin {Name}.", manager.Manifest.Name);

            // Delete directory
            manager.BaseDirectory.Delete(true);
            _plugins.Remove(manager.Manifest.Name);

            return true;
        }

        private bool DownloadAndInstall(IEnumerable<string> packageIds)
        {
            foreach (var packageId in packageIds)
            {
                _logger.Information("Downloading package {PackageId}", packageId);

                var package = _packageDownloader.Download(packageId);
                if (package == null)
                {
                    _logger.Error("Package {PackageId} was not found. Aborting...", packageId);
                    return false;
                }

                _packageInstaller.Install(package);
            }

            return true;
        }

        private void Load(IPluginManager manager)
        {
            var deps = _plugins.GetLoadOrder(manager.Manifest.Name).TakeWhile(s => s != manager.Manifest.Name);

            foreach (var dep in deps)
            {
                Load(dep);
            }

            manager.Load();
        }

        private void Unload(IPluginManager manager)
        {
            var deps = _plugins.GetUnloadOrder(manager.Manifest.Name).TakeWhile(s => s != manager.Manifest.Name);

            foreach (var dep in deps)
            {
                Unload(dep);
            }

            manager.Unload();
        }
    }
}
