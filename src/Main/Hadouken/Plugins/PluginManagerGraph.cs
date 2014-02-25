using System;
using System.Collections.Generic;
using System.Linq;

namespace Hadouken.Plugins
{
    internal sealed class PluginManagerGraph
    {
        private readonly IDictionary<string, IPluginManager> _plugins =
            new Dictionary<string, IPluginManager>(StringComparer.InvariantCultureIgnoreCase);
        private readonly object _pluginsLock = new object();
        private readonly DirectedGraph<string> _pluginsGraph = new DirectedGraph<string>();
        private readonly object _pluginsGraphLock = new object();

        public void AddRange(IEnumerable<IPluginManager> plugins)
        {
            foreach (var plugin in plugins)
            {
                lock (_pluginsLock)
                {
                    _plugins.Add(plugin.Manifest.Name, plugin);
                }
            }
        }

        public IEnumerable<IPluginManager> GetAll()
        {
            lock (_pluginsLock)
            {
                return _plugins.Values;
            }
        } 

        public void Rebuild()
        {
            foreach (var name in GetKeys())
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

        public string[] GetLoadOrder(string name)
        {
            lock (_pluginsGraphLock)
            {
                return _pluginsGraph.TraverseReverseOrder(name).ToArray();
            }
        }

        public string[] GetKeys()
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
