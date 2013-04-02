using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hadouken.IO;
using Hadouken.Configuration;

namespace Hadouken.Plugins.PluginEngine
{
    public class DefaultPluginEngine : IPluginEngine
    {
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
            // For each plugin in Paths.Plugin, call Load(path)
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

            // Using the loader, get all assemblies as a List<byte[]> (assemblies).

            // Create a new instance of SandboxedPlugin (sandbox) which should act as a proxy to the real plugin

            // Create a new instance of the SandboxedPluginManager (sandboxManager) which just makes WCF-calls to
            // the SandboxedPlugin to get information

            // Add the sandboxManager to the dictionary of IPluginManagers with the plugins name as key
        }

        public void UnloadAll()
        {
            throw new NotImplementedException();
        }

        public IDictionary<string, IPluginManager> Managers
        {
            get { return _pluginManagers; }
        }
    }
}
