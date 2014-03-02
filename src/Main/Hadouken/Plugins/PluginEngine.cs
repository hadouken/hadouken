using System;
using System.Collections.Generic;
using System.Linq;
using Hadouken.Plugins.Repository;
using NLog;

namespace Hadouken.Plugins
{
    public class PluginEngine : IPluginEngine
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly PluginManagerGraph _plugins = new PluginManagerGraph();

        private readonly IEnumerable<IPluginScanner> _pluginScanners;
        private readonly IPackageInstaller _packageInstaller;
        private readonly IPackageDownloader _packageDownloader;

        public PluginEngine(IEnumerable<IPluginScanner> pluginScanners, IPackageInstaller packageInstaller, IPackageDownloader packageDownloader)
        {
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

        public void Scan()
        {
            // Select all plugins from all scanners.
            var plugins = (from scanner in _pluginScanners
                from plugin in scanner.Scan()
                where Get(plugin.Manifest.Name) == null
                select plugin).ToList();

            Logger.Debug("Scan found {0} plugins to add", plugins.Count);

            // Add the new plugins
            _plugins.AddRange(plugins);
            _plugins.Rebuild();
        }

        public void LoadAll()
        {
            var managerKeys = _plugins.GetKeys();

            foreach (var key in managerKeys)
            {
                Load(key);
            }
        }

        public void UnloadAll()
        {
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
                Logger.Debug("Plugin does not exist: '{0}'", name);
                return;
            }

            // Check state
            if (manager.State != PluginState.Unloaded)
            {
                Logger.Info("Plugin cannot be loaded since it is not unloaded: '{0}' [state '{1}']", name, manager.State);
                return;
            }

            string[] missingDependencies;
            if (!CanLoad(name, out missingDependencies))
            {
                Logger.Warn("Plugin is missing dependencies. Downloading... [plugin '{0}'], [deps '{1}']", name,
                    string.Join(",", missingDependencies));

                if (DownloadAndInstall(missingDependencies))
                {
                    Scan();
                    Load(name);
                }
                return;
            }

            Load(manager);
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
                Logger.Debug("Plugin does not exist: '{0}'", name);
                return;
            }

            // Check state
            if (manager.State != PluginState.Loaded)
            {
                Logger.Info("Plugin cannot be unloaded since it is not loaded: '{0}' [state '{1}']", name, manager.State);
                return;
            }

            Unload(manager);
        }

        public bool InstallOrUpgrade(IPackage package)
        {
            // Check it we have a plugin with this name already. If we do,
            // we must have a higher version number 
            // the existing to be able to upgrade. Otherwise - out of luck.
            var existing = Get(package.Manifest.Name);
            if (existing != null)
            {
                if (existing.Manifest.Version >= package.Manifest.Version)
                {
                    Logger.Error("Downgrades not supported ({0} v{1}).", package.Manifest.Name, package.Manifest.Version);
                    return false;
                }

                Unload(existing);
                Uninstall(existing);
            }

            // Install dat package!
            Logger.Info("Installing package {0} v{1}", package.Manifest.Name, package.Manifest.Version);
            _packageInstaller.Install(package);

            Scan();
            Load(package.Manifest.Name);

            return true;
        }

        public bool Uninstall(string name)
        {
            var manager = Get(name);
            if (manager == null)
            {
                Logger.Debug("Plugin does not exist: '{0}'", name);
                return false;
            }

            return Uninstall(manager);
        }

        private bool Uninstall(IPluginManager manager)
        {
            // Check dependencies
            var dependencies = _plugins.GetUnloadOrder(manager.Manifest.Name);
            if (dependencies.Any(d => d != manager.Manifest.Name))
            {
                Logger.Error("Cannot uninstall plugin {0}. Plugins {1} still depend on it.", manager.Manifest.Name,
                    string.Join(",", dependencies));
                return false;
            }

            if (manager.State != PluginState.Unloaded)
            {
                Logger.Info("Tried to uninstall plugin that was not unloaded.");
                return false;
            }

            Logger.Info("Uninstalling existing plugin '{0}'.", manager.Manifest.Name);

            // Delete directory
            manager.BaseDirectory.Delete(true);
            _plugins.Remove(manager.Manifest.Name);

            return true;
        }

        private bool DownloadAndInstall(IEnumerable<string> packageIds)
        {
            foreach (var packageId in packageIds)
            {
                Logger.Info("Downloading package '{0}'", packageId);

                var package = _packageDownloader.Download(packageId);
                if (package == null)
                {
                    Logger.Warn("Package '{0}' was not found. Aborting...", packageId);
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
