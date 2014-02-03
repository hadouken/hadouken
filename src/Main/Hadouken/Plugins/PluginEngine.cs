using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Hadouken.Configuration;
using Hadouken.Framework;
using Hadouken.Framework.IO;
using Hadouken.Framework.IO.Local;
using Hadouken.Framework.Rpc;
using NLog;

namespace Hadouken.Plugins
{
    public class PluginEngine : IPluginEngine
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly IConfiguration _configuration;
        private readonly IFileSystem _fileSystem;
        private readonly IJsonRpcClient _rpcClient;

        private readonly IDictionary<string, IPluginManager> _pluginManagers =
            new Dictionary<string, IPluginManager>(StringComparer.InvariantCultureIgnoreCase);
        private DirectedGraph<string> _pluginGraph = new DirectedGraph<string>(); 

        private readonly object _lock = new object();

        public PluginEngine(IConfiguration configuration, IFileSystem fileSystem, IJsonRpcClient rpcClient)
        {
            _configuration = configuration;
            _fileSystem = fileSystem;
            _rpcClient = rpcClient;
        }

        public IEnumerable<IPluginManager> GetAll()
        {
            lock (_lock)
            {
                return new List<IPluginManager>(_pluginManagers.Values);
            }
        }

        public IPluginManager Get(string name)
        {
            lock (_lock)
            {
                if (_pluginManagers.ContainsKey(name))
                    return _pluginManagers[name];

                return null;
            }
        }

        private IEnumerable<IPackage> LoadPackagesFromBaseDirectory()
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

        private IEnumerable<IPackage> LoadPackagesFromExplicitList()
        {
            var result = new List<IPackage>();

            foreach (PluginElement pluginElement in _configuration.Plugins)
            {
                var directory = _fileSystem.GetDirectory(pluginElement.Path);

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
                    var file = _fileSystem.GetFile(pluginElement.Path);

                    if (!Package.TryParse(file, out package))
                    {
                        continue;
                    }
                }

                result.Add(package);
            }

            return result;
        } 

        public void Scan()
        {
            var packages = new List<IPackage>();
            packages.AddRange(LoadPackagesFromBaseDirectory());
            packages.AddRange(LoadPackagesFromExplicitList());

            foreach (var package in packages)
            {
                lock (_lock)
                {
                    if (_pluginManagers.ContainsKey(package.Manifest.Name))
                        continue;

                    var pluginDataPath = Path.Combine(_configuration.ApplicationDataPath, package.Manifest.Name);
                        
                    var dataDirectory = _fileSystem.GetDirectory(pluginDataPath);
                    dataDirectory.CreateIfNotExists();

                    var bootConfig = new BootConfig
                    {
                        DataPath = pluginDataPath,
                        HostBinding = _configuration.Http.HostBinding,
                        Port = _configuration.Http.Port,
                        UserName = _configuration.Http.Authentication.UserName,
                        Password = _configuration.Http.Authentication.Password,
                        RpcGatewayUri = _configuration.Rpc.GatewayUri,
                        RpcPluginUri = String.Format(_configuration.Rpc.PluginUri, package.Manifest.Name),
                        HttpVirtualPath = "/plugins/" + package.Manifest.Name
                    };

                    var manager = new PluginManager(package, bootConfig, _rpcClient);
                    _pluginManagers.Add(package.Manifest.Name, manager);
                }
            }

            var graph = new DirectedGraph<string>();

            foreach (var pluginManager in _pluginManagers)
            {
                graph.Add(pluginManager.Value.Package.Manifest.Name);
            }

            foreach (var pluginManager in _pluginManagers)
            {
                foreach (var dependency in pluginManager.Value.Package.Manifest.Dependencies)
                {
                    graph.Connect(dependency.Name, pluginManager.Value.Package.Manifest.Name);
                }
            }

            _pluginGraph = graph;
        }

        public Task ScanAsync()
        {
            return Task.Factory.StartNew(Scan);
        }

        public void Load(string name)
        {
            // Load the plugin manager specified by the name.
            // This will also load all of its dependencies.
            IPluginManager manager = null;
            
            lock (_lock)
            {
                if (_pluginManagers.ContainsKey(name))
                {
                    manager = _pluginManagers[name];
                }
            }

            if (manager == null || manager.State != PluginState.Unloaded)
                return;

            try
            {
                // Traverse all dependencies up to root objects and make sure all of them exist and are loaded.
                if (manager.Package.Manifest.Dependencies.Any())
                    LoadDependencies(manager);

                manager.Load();
            }
            catch (Exception exception)
            {
                Logger.Error(
                    String.Format("Error when loading plugin {0}[{1}].", manager.Package.Manifest.Name, manager.Package.Manifest.Version),
                    exception);
            }
        }

        private void LoadDependencies(IPluginManager manager)
        {
            // For each dependency, find a suitable plugin manager
            // and load those dependencies
            foreach (var dependency in manager.Package.Manifest.Dependencies)
            {
                IPluginManager managerDependency = null;

                lock (_lock)
                {
                    if (_pluginManagers.ContainsKey(dependency.Name))
                        managerDependency = _pluginManagers[dependency.Name];
                }

                // If we could not find this dependency at all, throw an error.
                if (managerDependency == null)
                    throw new DependencyNotFoundException("Dependency not found: " + dependency.Name);

                // If the dependency we found is the wrong version, throw an error.
                if (!dependency.VersionRange.IsIncluded(managerDependency.Package.Manifest.Version))
                    throw new InvalidDependencyVersionException(
                        String.Format("No valid version for dependency {0} found", dependency.Name));

                // If we got this far, load all dependencies for this plugin as well
                if (managerDependency.Package.Manifest.Dependencies.Any())
                    LoadDependencies(managerDependency);

                // Load the dependency
                if (managerDependency.State == PluginState.Unloaded)
                    managerDependency.Load();
            }
        }

        public Task LoadAsync(string name)
        {
            return Task.Factory.StartNew(() => Load(name));
        }

        public void LoadAll()
        {
            string[] names;

            lock (_lock)
            {
                names = _pluginManagers.Keys.ToArray();
            }

            foreach(var name in names)
            {
                Load(name);
            }
        }

        public Task LoadAllAsync()
        {
            return Task.Factory.StartNew(LoadAll);
        }

        public void Unload(string name)
        {
            IPluginManager manager = null;

            lock (_lock)
            {
                if (_pluginManagers.ContainsKey(name))
                    manager = _pluginManagers[name];
            }

            if (manager == null)
                return;

            try
            {
                manager.Unload();
            }
            catch (Exception exception)
            {
                Logger.ErrorException(
                    String.Format("Error when unloading plugin {0}[{1}].", manager.Package.Manifest.Name,
                                  manager.Package.Manifest.Version),
                    exception);
            }
        }

        public Task UnloadAsync(string name)
        {
            return Task.Factory.StartNew(() => Unload(name));
        }

        public void InstallOrUpgrade(IPackage package)
        {
            // Get the existing package/plugin if we have any
            var existing = Get(package.Manifest.Name);

            // If the existing version is newer than the provided one, return
            if (existing != null && existing.Package.Manifest.Version >= package.Manifest.Version)
            {
                return;
            }

            // If the existing one is older than the uploaded one, unload and remove
            if (existing != null && package.Manifest.Version > existing.Package.Manifest.Version)
            {
                var unloadOrder = _pluginGraph.TraverseReverseOrder(existing.Package.Manifest.Name);

                if (existing.Package.Manifest.Name != unloadOrder.Last())
                {
                    throw new InvalidOperationException();
                }

                // Unload existing plugins
                foreach (var name in unloadOrder)
                {
                    Unload(name);                    
                }

                // Remove it from the plugin engine
                Remove(existing.Package.Manifest.Name);

                // Remove it from disk
                var existingPath = Path.Combine(_configuration.Plugins.BaseDirectory,
                    existing.Package.Manifest.Name + ".zip");
                var file = _fileSystem.GetFile(existingPath);

                file.Delete();
            }

            // Save new package to default plugin location
            var fileName = String.Concat(package.Manifest.Name, ".zip");
            var path = Path.Combine(_configuration.Plugins.BaseDirectory, fileName);

            File.WriteAllBytes(path, package.Data);

            // Scan for new plugins
            Scan();

            // Load the newly uploaded plugin
            var loadOrder = _pluginGraph.TraverseReverseOrder(package.Manifest.Name);
            loadOrder.Reverse();

            foreach (var name in loadOrder)
            {
                Load(name);
            }
        }

        public void UnloadAll()
        {
            string[] names;

            lock (_lock)
            {
                names = _pluginManagers.Keys.ToArray();
            }

            foreach (var name in names)
            {
                Unload(name);
            }
        }

        public Task UnloadAllAsync()
        {
            return Task.Factory.StartNew(UnloadAll);
        }

        public void Remove(string name)
        {
            IPluginManager manager = null;

            lock (_lock)
            {
                if (_pluginManagers.ContainsKey(name))
                    manager = _pluginManagers[name];
            }

            if (manager == null || manager.State != PluginState.Unloaded)
                return;

            lock (_lock)
            {
                _pluginManagers.Remove(name);
            }
        }

        public Task RemoveAsync(string name)
        {
            return Task.Factory.StartNew(() => Remove(name));
        }

        public void Unload()
        {
            lock (_lock)
            {
                foreach (var manager in _pluginManagers.Values)
                {
                    Logger.Info("Unloading plugin {0}", manager.Package.Manifest.Name);

                    try
                    {
                        manager.Unload();
                    }
                    catch (Exception e)
                    {
                        Logger.ErrorException(String.Format("Could not unload plugin {0}", manager.Package.Manifest.Name), e);
                    }
                }

                _pluginManagers.Clear();
            }
        }
    }
}
