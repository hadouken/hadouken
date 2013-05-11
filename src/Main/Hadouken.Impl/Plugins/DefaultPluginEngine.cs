using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hadouken.Plugins;
using Hadouken.Data;
using Hadouken.Data.Models;
using Hadouken.Messaging;
using Hadouken.Configuration;
using Hadouken.IO;
using Hadouken.Reflection;
using NLog;

namespace Hadouken.Impl.Plugins
{
    [Component]
    public class DefaultPluginEngine : IPluginEngine
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private Dictionary<string, IPluginManager> _managers = new Dictionary<string, IPluginManager>(StringComparer.InvariantCultureIgnoreCase);

        private IFileSystem _fs;
        private IDataRepository _repo;
        private IMessageBus _mbus;
        private IMigrationRunner _runner;
        private IPluginLoader[] _loaders;

        public DefaultPluginEngine(IFileSystem fs, IDataRepository repo, IMessageBus mbus, IMigrationRunner runner, IPluginLoader[] loaders)
        {
            _fs = fs;
            _repo = repo;
            _mbus = mbus;
            _runner = runner;
            _loaders = loaders;
        }

        public void Load()
        {
            var pluginInfos = _repo.List<PluginInfo>();
            var path = HdknConfig.GetPath("Paths.Plugins");

            foreach (var info in _fs.GetFileSystemInfos(path)
                                    .Select(i => i.FullName)
                                    .Union(pluginInfos.Select(pi => pi.Path))
                                    .Distinct())
            {
                Load(info);
            }
        }

        public void Load(string path)
        {
            
            // Do we have a IPluginLoader for this path?
            var loader = _loaders.FirstOrDefault(l => l.CanLoad(path));

            if (loader == null)
            {
                Logger.Info("No IPluginLoader can handle the path {0}", path);
                return;
            }

            var pluginTypes = loader.Load(path);

            foreach (var pluginType in pluginTypes)
            {
                Load(path, pluginType);
            }
        }

        internal void Load(string path, Type pluginType)
        {
            var attr = pluginType.GetAttribute<PluginAttribute>();

            // Plugin not marked with attribute, should
            // not happen since the IPluginLoader should've
            // checked that it exists.
            if (attr == null)
                return;

            // We have already loaded this plugin, do nothing
            if (_managers.ContainsKey(attr.Name))
                return;

            var manager = new DefaultPluginManager(pluginType, _mbus, _runner);
            manager.Initialize();

            // Check if we have a PluginInfo for this plugin.
            // If we don't, add one and run Install()
            // If we do, and versions are NOT equal, run Install()
            // If we do, and versions are equal, do nothing

            var pluginInfo = _repo.Single<PluginInfo>(p => p.Name == attr.Name);

            if (pluginInfo == null)
            {
                _repo.Save(new PluginInfo
                {
                    Name = attr.Name,
                    Version = attr.Version.ToString(),
                    Path = path
                });

                manager.Install();

                Logger.Info("Found new plugin ({0} [{1}])", attr.Name, attr.Version);
            }
            else if (new Version(pluginInfo.Version) < attr.Version || String.IsNullOrEmpty(pluginInfo.Version))
            {
                string oldVersion = pluginInfo.Version;

                manager.Install();

                pluginInfo.Name = manager.Name;
                pluginInfo.Version = manager.Version.ToString();

                _repo.Update(pluginInfo);

                Logger.Info("Plugin {0} upgraded from [{1}] to [{2}]", manager.Name, oldVersion, attr.Version);
            }

            Load(manager);
        }

        internal void Load(IPluginManager manager)
        {
            manager.Load();
            _managers.Add(manager.Name, manager);

            Logger.Info("{0} [{1}] loaded", manager.Name, manager.Version);
        }

        public void UnloadAll()
        {
            var managers = _managers.Values;

            foreach (var man in managers)
            {
                try
                {
                    man.Unload();
                }
                catch(Exception e)
                {
                    Logger.ErrorException(String.Format("Could not unload plugin {0}", man.Name), e);
                }
            }
        }

        public IDictionary<string, IPluginManager> Managers
        {
            get { return _managers; }
        }
    }
}
