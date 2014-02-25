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

        private readonly IDictionary<string, IPluginManager> _plugins =
            new Dictionary<string, IPluginManager>(StringComparer.InvariantCultureIgnoreCase); 
        private readonly object _pluginsLock = new object();
        private readonly DirectedGraph<string> _pluginsGraph = new DirectedGraph<string>(); 
        private readonly object _pluginsGraphLock = new object();

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
            lock (_pluginsLock)
            {
                return _plugins.Values;
            }
        }

        public IPluginManager Get(string name)
        {
            lock (_pluginsLock)
            {
                if (_plugins.ContainsKey(name))
                {
                    return _plugins[name];
                }
            }

            return null;
        }

        public void Scan()
        {
            // Select all plugins from all scanners.
            var plugins = (from scanner in _pluginScanners
                from plugin in scanner.Scan()
                where Get(plugin.Manifest.Name) == null
                select plugin).ToList();

            // Add the new plugins
            foreach (var plugin in plugins)
            {
                lock (_pluginsLock)
                {
                    _plugins.Add(plugin.Manifest.Name, plugin);
                }
            }

            RebuildGraph();
        }

        public void LoadAll()
        {
            var managerKeys = GetManagerKeys();

            foreach (var key in managerKeys)
            {
                Load(key);
            }
        }

        public void UnloadAll()
        {
            var managerKeys = GetManagerKeys();

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
                Logger.Debug("Load was called with invalid key: {0}", name);
                return;
            }

            // Check state
            if (manager.State != PluginState.Unloaded)
            {
                Logger.Info("PluginManager not in correct state, manager:{0}, state:{1}", name, manager.State);
                return;
            }

            string[] missingDependencies;
            if (!CanLoad(name, out missingDependencies))
            {
                Logger.Warn("Unmet dependencies for plugin {0}, ({1}). Will try to download.", name, string.Join(",", missingDependencies));

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
            missingDependencies = null;
            string[] dependencies;

            lock (_pluginsGraphLock)
            {
                dependencies = _pluginsGraph.TraverseReverseOrder(name).ToArray();
            }

            var missingDeps = dependencies.Where(dependency => Get(dependency) == null).ToArray();

            if (missingDeps.Any())
            {
                missingDependencies = missingDeps;
                return false;
            }

            return true;
        }

        public void Unload(string name)
        {
            throw new NotImplementedException();
        }

        public void InstallOrUpgrade(IPackage package)
        {
            throw new NotImplementedException();
        }

        public void Uninstall(string name)
        {
            throw new NotImplementedException();
        }

        private bool DownloadAndInstall(IEnumerable<string> packageIds)
        {
            foreach (var packageId in packageIds)
            {
                var package = _packageDownloader.Download(packageId);

                if (package == null)
                {
                    return false;
                }

                _packageInstaller.Install(package);
            }

            return true;
        }

        private void Load(IPluginManager manager)
        {
            var deps =
                _pluginsGraph.TraverseReverseOrder(manager.Manifest.Name).TakeWhile(s => s != manager.Manifest.Name);

            foreach (var dep in deps)
            {
                Load(dep);
            }

            manager.Load();
        }

        private void RebuildGraph()
        {
            foreach (var name in GetManagerKeys())
            {
                var manager = Get(name);

                if (manager == null)
                {
                    continue;
                }

                foreach (var dependency in manager.Manifest.Dependencies)
                {
                    lock (_pluginsGraphLock)
                    {
                        _pluginsGraph.Connect(name, dependency.Name);
                    }
                }
            }
        }

        private string[] GetManagerKeys()
        {
            string[] managerNames;

            lock (_pluginsLock)
            {
                managerNames = _plugins.Keys.ToArray();
            }

            return managerNames;
        }
    }
}
