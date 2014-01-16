using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Hadouken.Configuration;
using Hadouken.Framework;
using Hadouken.Framework.IO;
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

        public void Scan()
        {
            var pluginDirectory = _fileSystem.GetDirectory(_configuration.Plugins.BaseDirectory);

            foreach (var file in pluginDirectory.Files)
            {
                IPackage package;

                if (!Package.TryParse(file.FullPath, out package))
                    continue;

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

        private static string GetOptionalPluginArgument(string[] args)
        {
            // Args cannot be null and its length must be 2 or more
            // since --plugin and "path" sits in two different positions.

            if (args == null || args.Length < 2)
                return null;

            // Args must contain --plugin

            if (!args.Contains("--plugin"))
                return null;

            var position = Array.IndexOf(args, "--plugin");

            // The array must be at least one field longer
            // than the position of the --plugin entry

            if (position == -1 || args.Length - 1 < position + 1)
                return null;

            return args[position + 1];
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
