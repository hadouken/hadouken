using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Hadouken.Configuration;
using Hadouken.Framework.IO;
using Hadouken.Plugins.Metadata;
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

        private readonly IConfiguration _configuration;
        private readonly IFileSystem _fileSystem;
        private readonly IPluginManagerFactory _managerFactory;

        public PluginEngine(IConfiguration configuration, IFileSystem fileSystem, IPluginManagerFactory managerFactory)
        {
            _configuration = configuration;
            _fileSystem = fileSystem;
            _managerFactory = managerFactory;
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
            var baseDirectory = _fileSystem.GetDirectory(_configuration.Plugins.BaseDirectory);
            var pluginDirectories = baseDirectory.Directories;

            foreach (var directory in pluginDirectories)
            {
                // Find manifest file
                var manifestFile = directory.Files.SingleOrDefault(f => f.Name == Manifest.FileName);
                if (manifestFile == null || !manifestFile.Exists)
                {
                    continue;
                }

                using (var manifestStream = manifestFile.OpenRead())
                {
                    IManifest manifest;
                    if (!Manifest.TryParse(manifestStream, out manifest))
                    {
                        continue;
                    }

                    // Check if we already have this plugin
                    if (Get(manifest.Name) != null)
                    {
                        continue;
                    }

                    // New plugin, create the plugin manager
                    var manager = _managerFactory.Create(directory, manifest);
                    lock (_pluginsLock)
                    {
                        _plugins.Add(manager.Manifest.Name, manager);
                    }
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
                Logger.Error(
                    "Plugin {0} cannot load. Unmet dependencies: {1}.",
                    name,
                    string.Join(",", missingDependencies));
            }

            Load(manager);
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
