using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NuGet;
using Directory = Hadouken.Fx.IO.Directory;
using ILogger = Serilog.ILogger;

namespace Hadouken.Plugins
{
    public class PluginEngine : IPluginEngine
    {
        private readonly PluginManagerGraph _plugins = new PluginManagerGraph();

        private readonly ILogger _logger;
        private readonly IPackageManager _packageManager;
        private readonly IDevelopmentPluginInstaller _devPluginInstaller;
        private readonly IPluginManagerFactory _pluginManagerFactory;
        private bool _checkedForDevPackage;

        public PluginEngine(ILogger logger, IPackageManager packageManager, IDevelopmentPluginInstaller devPluginInstaller, IPluginManagerFactory pluginManagerFactory)
        {
            if (logger == null) throw new ArgumentNullException("logger");
            if (packageManager == null) throw new ArgumentNullException("packageManager");
            if (devPluginInstaller == null) throw new ArgumentNullException("devPluginInstaller");
            if (pluginManagerFactory == null) throw new ArgumentNullException("pluginManagerFactory");

            _logger = logger;
            _packageManager = packageManager;
            _devPluginInstaller = devPluginInstaller;
            _pluginManagerFactory = pluginManagerFactory;
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

        public void Refresh()
        {
            RefreshDev();

            var packages = _packageManager.LocalRepository.GetPackages().ToList();
            foreach (var package in packages)
            {
                var directory = _packageManager.PathResolver.GetInstallPath(package);
                var manager = _pluginManagerFactory.Create(new Directory(new DirectoryInfo(directory)), package);

                if (_plugins.Get(package.Id) != null)
                {
                    _plugins.Add(manager);
                }
            }

            _plugins.Rebuild();
        }

        private void RefreshDev()
        {
            if (_checkedForDevPackage) return;

            var pkg = _devPluginInstaller.GetPackage();

            if (pkg != null)
            {
                // If we have a package with this ID installed, uninstall it.
                var existing = _packageManager.LocalRepository.FindPackage(pkg.Id);
                if (existing != null)
                {
                    _logger.Information("Uninstalling old development package {PackageId}.", pkg.Id);
                    _packageManager.UninstallPackage(existing, true, true);
                }

                try
                {
                    _packageManager.InstallPackage(pkg, false, true);
                }
                catch (InvalidOperationException exception)
                {
                    _logger.Error(exception, "Error when installing plugin {PluginId}", pkg.Id);
                }
            }

            _checkedForDevPackage = true;
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

            Load(manager);
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

        public void Install(string packageId, string version, bool ignoreDependencies, bool allowPrereleaseVersions)
        {
            var semver = SemanticVersion.Parse(version);
            _packageManager.InstallPackage(packageId, semver, ignoreDependencies, allowPrereleaseVersions);
            
            // Load the installed package
            Refresh();
            Load(packageId);
        }

        public void Uninstall(string packageId, string version, bool forceRemove, bool removeDependencies)
        {
            // Unload the plugin
            Unload(packageId);

            _logger.Information("Uninstalling {PackageId}.", packageId);

            // Uninstall it
            var semver = SemanticVersion.Parse(version);
            _packageManager.UninstallPackage(packageId, semver, forceRemove, removeDependencies);

            // Refresh the plugin list
            _plugins.Remove(packageId);
            _plugins.Rebuild();
        }

        public void Update(string packageId, string version, bool updateDependencies, bool allowPrereleaseVersions)
        {
            var semver = NuGet.SemanticVersion.Parse(version);
            _packageManager.UpdatePackage(packageId, semver, updateDependencies, allowPrereleaseVersions);
        }

        private void Load(IPluginManager manager)
        {
            var deps = _plugins.GetLoadOrder(manager.Package.Id).TakeWhile(s => s != manager.Package.Id);

            foreach (var dep in deps)
            {
                Load(dep);
            }

            manager.Load();
        }

        private void Unload(IPluginManager manager)
        {
            var deps = _plugins.GetUnloadOrder(manager.Package.Id).TakeWhile(s => s != manager.Package.Id);

            foreach (var dep in deps)
            {
                Unload(dep);
            }

            manager.Unload();
        }
    }
}
