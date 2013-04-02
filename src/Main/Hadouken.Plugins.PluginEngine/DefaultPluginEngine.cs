using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hadouken.IO;
using Hadouken.Configuration;
using NLog;

namespace Hadouken.Plugins.PluginEngine
{
    public class DefaultPluginEngine : IPluginEngine
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly IDictionary<string, IPluginManager> _pluginManagers = new Dictionary<string, IPluginManager>(StringComparer.InvariantCultureIgnoreCase);

        private readonly IFileSystem _fileSystem;
        private readonly IPluginLoader[] _pluginLoaders;

        public DefaultPluginEngine(IFileSystem fileSystem, IPluginLoader[] pluginLoaders)
        {
            _fileSystem = fileSystem;
            _pluginLoaders = pluginLoaders;
        }

        public void Load()
        {
            var pluginPath = HdknConfig.GetPath("Paths.Plugins");

            foreach (var info in _fileSystem.GetFileSystemInfos(pluginPath))
            {
                Load(info.FullName);
            }
        }

        public void Load(string path)
        {
            var loader = (from l in _pluginLoaders
                          where l.CanLoad(path)
                          select l).SingleOrDefault();

            if (loader == null)
                return;

            Logger.Debug("Loading plugin from path '{0}' using loader '{1}'", path, loader.GetType().FullName);

            var assemblies = loader.Load(path);

            var domain = AppDomain.CreateDomain(path);
            var sandbox = (SandboxedPlugin)domain.CreateInstanceFromAndUnwrap(this.GetType().Assembly.Location,
                                                             typeof (SandboxedPlugin).FullName);

            sandbox.LoadAssemblies(assemblies);

            var manager = new SandboxedPluginManager(sandbox);
            manager.Load();

            _pluginManagers.Add(manager.Name, manager);
        }

        public void UnloadAll()
        {
            var managers = _pluginManagers.Values;

            foreach (var manager in managers)
            {
                manager.Unload();
                _pluginManagers.Remove(manager.Name);
            }
        }

        public IDictionary<string, IPluginManager> Managers
        {
            get { return _pluginManagers; }
        }
    }
}
