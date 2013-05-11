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
using System.Reflection;

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

        public DefaultPluginEngine(IFileSystem fs,
                                   IDataRepository repo,
                                   IMessageBus mbus,
                                   IMigrationRunner runner,
                                   IPluginLoader[] loaders)
        {
            _fs = fs;
            _repo = repo;
            _mbus = mbus;
            _runner = runner;
            _loaders = loaders;
        }

        public void Load()
        {
            var path = HdknConfig.GetPath("Paths.Plugins");

            foreach (var info in _fs.GetFileSystemInfos(path)
                                    .Select(i => i.FullName))
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

            var assemblies = loader.Load(path);
            Load(assemblies);
        }

        public void Load(IEnumerable<byte[]> rawAssemblies)
        {
            var assemblies = rawAssemblies.Select(Assembly.Load).ToList();

            // Run migrations
            foreach (var asm in assemblies)
            {
                _runner.Up(asm);
            }

            var childResolver = Kernel.Resolver.CreateChildResolver(assemblies);
            var plugin = childResolver.Get<IPlugin>();

            var manager = new DefaultPluginManager(plugin);
            manager.Load();

            _managers.Add(manager.Name, manager);
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
