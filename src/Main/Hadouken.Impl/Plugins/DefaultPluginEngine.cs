using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hadouken.Plugins;
using Hadouken.Data;
using Hadouken.Data.Models;
using Hadouken.Messaging;

namespace Hadouken.Impl.Plugins
{
    public class DefaultPluginEngine : IPluginEngine
    {
        private Dictionary<string, IPluginManager> _managers = new Dictionary<string, IPluginManager>(StringComparer.InvariantCultureIgnoreCase);

        private IDataRepository _repo;
        private IMessageBus _mbus;
        private IMigrationRunner _runner;
        private IPluginLoader[] _loaders;

        public DefaultPluginEngine(IDataRepository repo, IMessageBus mbus, IMigrationRunner runner, IPluginLoader[] loaders)
        {
            _repo = repo;
            _mbus = mbus;
            _runner = runner;
            _loaders = loaders;
        }

        public IEnumerable<IPluginManager> Refresh()
        {
            // load all PluginInfo where Name not in _managers.Keys
            var infos = _repo.List<PluginInfo>();

            foreach (PluginInfo info in infos)
            {
                var manager = new DefaultPluginManager(info, _mbus, _runner, _loaders);

                if (!_managers.ContainsKey(manager.Name))
                {
                    manager.Initialize();
                    manager.Install();

                    _managers.Add(manager.Name, manager);

                    yield return manager;
                }
            }
        }

        public void LoadAll()
        {
            foreach (var man in _managers.Values)
                man.Load();
        }

        public void UnloadAll()
        {
            foreach (var man in _managers.Values)
                man.Unload();
        }

        public IDictionary<string, IPluginManager> Managers
        {
            get { return _managers; }
        }
    }
}
