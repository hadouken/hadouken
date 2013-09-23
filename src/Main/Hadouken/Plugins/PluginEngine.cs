using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hadouken.Configuration;
using Hadouken.Framework;
using Hadouken.IO;
using NLog;
using System.IO;

namespace Hadouken.Plugins
{
    public class PluginEngine : IPluginEngine
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly IConfiguration _configuration;
        private readonly IFileSystem _fileSystem;
        private readonly IEnumerable<IPluginLoader> _pluginLoaders;

        private readonly IDictionary<string, IPluginManager> _pluginManagers =
            new Dictionary<string, IPluginManager>(StringComparer.InvariantCultureIgnoreCase);

        private readonly object _lock = new object();

        public PluginEngine(IConfiguration configuration, IFileSystem fileSystem, IEnumerable<IPluginLoader> pluginLoaders)
        {
            _configuration = configuration;
            _fileSystem = fileSystem;
            _pluginLoaders = pluginLoaders;
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

        public void Load()
        {
            var entries = new List<string>((from PluginElement element
                                                in _configuration.Plugins
                                            select element.Path));

            foreach (var entry in entries)
            {
                Logger.Info("Loading plugin from {0}", entry);

                var loader = (from p in _pluginLoaders
                    where p.CanLoad(entry)
                    select p).FirstOrDefault();

                if (loader == null)
                {
                    Logger.Warn("Could not find a plugin loader for path {0}", entry);
                    continue;
                }

                IPluginManager manager;

                try
                {
                    manager = loader.Load(entry);
                }
                catch (Exception e)
                {
                    Logger.ErrorException(String.Format("Could not load plugin from path {0}", entry), e);
                    return;
                }

                LoadPluginManager(manager);
            }
        }

        private void LoadPluginManager(IPluginManager manager)
        {
            var config = new BootConfig
            {
                DataPath = Path.Combine(_configuration.ApplicationDataPath, "Plugins", manager.Name),
                HostBinding = _configuration.Http.HostBinding,
                Port = _configuration.Http.Port,
                ApiBaseUri = "api/" + manager.Name
            };

            manager.SetBootConfig(config);

            try
            {
                manager.Load();
            }
            catch (Exception e)
            {
                Logger.ErrorException(String.Format("Could not load plugin {0}", manager.Name), e);
                
                manager.Unload();
                return;
            }

            lock (_lock)
            {
                _pluginManagers.Add(manager.Name, manager);
            }

            Logger.Info("Plugin {0}[v{1}] loaded", manager.Name, manager.Version);
        }

        public void Unload()
        {
            lock (_lock)
            {
                foreach (var manager in _pluginManagers.Values)
                {
                    Logger.Info("Unloading plugin {0}", manager.Name);

                    try
                    {
                        manager.Unload();
                    }
                    catch (Exception e)
                    {
                        Logger.ErrorException(String.Format("Could not unload plugin {0}", manager.Name), e);
                    }
                }

                _pluginManagers.Clear();
            }
        }
    }
}
