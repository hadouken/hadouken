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
using NLog;

namespace Hadouken.Impl.Plugins
{
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

        public IEnumerable<IPluginManager> Refresh()
        {
            var infos = _repo.List<PluginInfo>();

            // Load all plugins from path
            var path = HdknConfig.GetPath("Paths.Plugins");

            foreach(var info in _fs.GetFileSystemInfos(path))
            {
                if(_loaders.Any(l => l.CanLoad(info.FullName)) && infos.All(i => i.Path != info.FullName))
                {
                    Logger.Info("Found new plugin {0}", info.FullName);

                    var pluginInfo = new PluginInfo {Path = info.FullName};
                    _repo.Save(pluginInfo);
                    infos.Add(pluginInfo);
                }
            }

            // load all PluginInfo where Name not in _managers.Keys
            var ret = new List<IPluginManager>();

            foreach (PluginInfo info in infos)
            {
                try
                {
                    var manager = new DefaultPluginManager(info, _mbus, _runner, _loaders);

                    if (!_managers.ContainsKey(manager.Name))
                    {
                        manager.Initialize();
                        manager.Install();

                        _managers.Add(manager.Name, manager);

                        ret.Add(manager);
                    }
                }
                catch(Exception e)
                {
                    Logger.ErrorException(String.Format("Could not create plugin from path {0}", info.Path), e);
                }
            }

            return ret;
        }

        public void LoadAll()
        {
            foreach (var man in _managers.Values)
            {
                try
                {
                    man.Load();
                }
                catch(Exception e)
                {
                    Logger.ErrorException(String.Format("Could not load plugin {0}", man.Name), e);
                }
            }
        }

        public void UnloadAll()
        {
            foreach (var man in _managers.Values)
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
