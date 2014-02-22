using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Hadouken.Configuration;
using Hadouken.Framework;
using Hadouken.Framework.IO;
using Hadouken.Framework.Rpc;
using Hadouken.Plugins.Isolation;
using NLog;

namespace Hadouken.Plugins
{
    public class PluginEngine : IPluginEngine
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly IConfiguration _configuration;
        private readonly IFileSystem _fileSystem;
        private readonly IJsonRpcClient _rpcClient;
        private readonly IPackageDownloader _packageDownloader;
        private readonly IPackageFactory _packageFactory;
        private readonly IIsolatedEnvironmentFactory _isolatedEnvironmentFactory;
        private readonly DirectedGraph<IPluginManager> _plugins = new DirectedGraph<IPluginManager>(); 
        private readonly object _lock = new object();

        public PluginEngine(IConfiguration configuration, IFileSystem fileSystem, IJsonRpcClient rpcClient, IPackageDownloader packageDownloader, IPackageFactory packageFactory, IIsolatedEnvironmentFactory isolatedEnvironmentFactory)
        {
            _configuration = configuration;
            _fileSystem = fileSystem;
            _rpcClient = rpcClient;
            _packageDownloader = packageDownloader;
            _packageFactory = packageFactory;
            _isolatedEnvironmentFactory = isolatedEnvironmentFactory;
        }

        public IEnumerable<IPluginManager> GetAll()
        {
            return Plugins.Nodes;
        }

        public IPluginManager Get(string name)
        {
            lock (_lock)
            {
                return
                    Plugins.Nodes.SingleOrDefault(
                        n => string.Equals(n.Package.Manifest.Name, name, StringComparison.InvariantCultureIgnoreCase));
            }
        }

        private DirectedGraph<IPluginManager> Plugins
        {
            get { return _plugins; }
        } 

        public void Rebuild()
        {
            var packages = _packageFactory.Scan();

            foreach (var package in packages)
            {
                AddPackage(package);
            }

            ConnectDependencies();
        }

        private void AddPackage(IPackage package)
        {
            // If we already have added this to the graph, skip it
            if (Get(package.Manifest.Name) != null)
                return;

            Logger.Info("Adding package {0}-{1}", package.Manifest.Name, package.Manifest.Version);

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
                RpcPluginUri = String.Format(_configuration.Rpc.PluginUriTemplate, package.Manifest.Name),
                HttpVirtualPath = "/plugins/" + package.Manifest.Name
            };

            var manager = new PluginManager(_isolatedEnvironmentFactory, package, bootConfig, _rpcClient);

            lock (_lock)
            {
                Plugins.Add(manager);
            }
        }

        private void ConnectDependencies()
        {
            Logger.Info("Rebuilding all dependencies.");

            // Disconnect all relationships.
            Plugins.DisconnectAll();

            // Create copy of list of nodes.
            var nodes = Plugins.Nodes.ToList();

            foreach (var manager in nodes)
            {
                foreach (var dependency in manager.Package.Manifest.Dependencies)
                {
                    var other = Get(dependency.Name);

                    if (other == null)
                    {
                        Logger.Error("Dependency {0} does not exist. Attempting download...", dependency.Name);

                        var package = _packageDownloader.Download(dependency.Name);

                        if (package == null)
                        {
                            Logger.Fatal("Download of dependency {0} failed.", dependency.Name);
                            throw new Exception("Dependency does not exist");
                        }

                        // Install dat missing dependency
                        InstallOrUpgrade(package);
                        other = Get(package.Manifest.Name);
                    }

                    if (dependency.VersionRange != null &&
                        !dependency.VersionRange.IsIncluded(other.Package.Manifest.Version))
                    {
                        Logger.Fatal("Dependency {0} has incorrect version {1}. Range was {2}",
                            dependency.Name,
                            other.Package.Manifest.Version,
                            dependency.VersionRange);

                        throw new Exception("Dependency has incorrect version");
                    }

                    Plugins.Connect(manager, other);
                }
            }
        }

        public void Load(string name)
        {
            // Get plugin with the specified name.
            var plugin = Get(name);

            // If it does not exist, return.
            if (plugin == null)
            {
                return;
            }

            // Get a list of all plugins we must load before this, and make sure they are loaded.
            var dependantPlugins = Plugins.TraverseReverseOrder(plugin);

            // Load 'em all!
            foreach (var dependantPlugin in dependantPlugins)
            {
                Load(dependantPlugin);
            }
        }

        public void LoadAll()
        {
            var pluginNames = Plugins.Nodes.Select(p => p.Package.Manifest.Name).ToArray();

            foreach (var name in pluginNames)
            {
                Load(name);
            }
        }

        public void Unload(string name)
        {
            // Get plugin with the specified name.
            var plugin = Get(name);

            // If it does not exist, return.
            if (plugin == null)
            {
                return;
            }

            // Get a list of all plugins we must load before this, and make sure they are loaded.
            var dependantPlugins = Plugins.TraverseOrder(plugin);

            // Load 'em all!
            foreach (var dependantPlugin in dependantPlugins)
            {
                Unload(dependantPlugin);
            }
        }

        private void Load(IPluginManager manager)
        {
            if (manager == null)
                return;

            if (manager.State != PluginState.Unloaded)
                return;

            manager.Load();
        }

        private void Unload(IPluginManager manager)
        {
            if (manager == null)
                return;

            if (manager.State != PluginState.Loaded)
                return;

            manager.Unload();
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

            Logger.Info("Installing/upgrading plugin {0}", package.Manifest.Name);

            // The order in which we unload plugins
            IList<IPluginManager> order = Plugins.TraverseOrder(existing);

            // If the existing one is older than the uploaded one, unload and remove
            if (existing != null && package.Manifest.Version > existing.Package.Manifest.Version)
            {
                Logger.Info("Unloading dependencies before removing old version.");

                // Unload existing plugins
                foreach (var name in order)
                {
                    Unload(name);                    
                }

                // Remove it from the plugin engine
                Remove(existing.Package.Manifest.Name);

                // Remove it from disk
                var packageFileName = string.Format("{0}-{1}.zip", existing.Package.Manifest.Name,
                    existing.Package.Manifest.Version);

                var existingPath = Path.Combine(_configuration.Plugins.BaseDirectory, packageFileName);
                var file = _fileSystem.GetFile(existingPath);

                Logger.Info("Removing package {0} from disk", file.FullPath);
                file.Delete();
            }

            // Save new package to default plugin location
            var fileName = string.Format("{0}-{1}.zip", package.Manifest.Name, package.Manifest.Version);
            var path = Path.Combine(_configuration.Plugins.BaseDirectory, fileName);
            var packageFile = _fileSystem.GetFile(path);

            Logger.Info("Saving package {0} to disk", packageFile.FullPath);

            using (var stream = packageFile.OpenWrite())
            using (var ms = new MemoryStream(package.Data))
            {
                ms.CopyTo(stream);
            }

            // Add new plugin manager
            AddPackage(package);
            var manager = Get(package.Manifest.Name);

            if (manager == null)
            {
                throw new Exception("Could not install plugin.");
            }

            // Rebuild the dependencies
            ConnectDependencies();

            var reverseOrder = new List<IPluginManager>();
            reverseOrder.Add(manager);
            reverseOrder.AddRange((from man in order
                where man != null
                let p = Get(man.Package.Manifest.Name)
                where p != null
                where man.Package.Manifest.Name != manager.Package.Manifest.Name
                select man));

            Logger.Info("Loading dependencies after upgrading.");

            // Reverse order and load 'em back up
            foreach (var dependantManager in reverseOrder)
            {
                Load(dependantManager);
            }
        }

        public void UnloadAll()
        {
            var pluginNames = Plugins.Nodes.Select(n => n.Package.Manifest.Name).ToArray();

            foreach (var name in pluginNames)
            {
                Unload(name);
            }
        }

        public void Remove(string name)
        {
            var plugin = Get(name);

            if (plugin == null)
                return;

            Logger.Info("Removing plugin {0} from graph.", name);

            // If it has no dependant nodes which are loaded, remove it.
            var dependantPlugins = Plugins.TraverseOrder(plugin);

            if (dependantPlugins.Any(p => p.State == PluginState.Loaded))
            {
                Logger.Fatal("Could not remove plugin {0} since the plugins {1} depends on it and are still loaded.",
                    string.Join(",", dependantPlugins.Select(p => p.Package.Manifest.Name)));

                throw new Exception("Cannot remove plugin which have loaded dependant nodes.");
            }

            Plugins.Remove(plugin);
        }
    }
}
