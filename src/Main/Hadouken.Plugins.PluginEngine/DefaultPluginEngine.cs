using System;
using System.Linq;
using NLog;
using Hadouken.Common;
using Hadouken.Configuration;
using Hadouken.Common.IO;
using Hadouken.Common.Messaging;
using Hadouken.Common.Plugins;

namespace Hadouken.Plugins.PluginEngine
{
    [Component]
    public class DefaultPluginEngine : IPluginEngine
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly IFileSystem _fileSystem;
        private readonly IMessageBus _messageBus;
        private readonly IPluginLoader[] _pluginLoaders;

        public DefaultPluginEngine(IFileSystem fileSystem,
                                   IMessageBusFactory messageBusFactory,
                                   IPluginLoader[] pluginLoaders)
        {
            _fileSystem = fileSystem;
            _messageBus = messageBusFactory.Create("hdkn.pluginEngine");
            _pluginLoaders = pluginLoaders;
        }

        public void Load()
        {
            var path = HdknConfig.GetPath("Paths.Plugins");

            foreach (var file in _fileSystem.GetFileSystemInfos(path))
            {
                Load(file.FullName);
            }
        }

        public void Load(string path)
        {
            var pluginLoader = (from pl in _pluginLoaders
                                where pl.CanLoad(path)
                                select pl).FirstOrDefault();

            if (pluginLoader == null)
            {
                Logger.Warn("No plugin loader available for path '{0}'.", path);
                return;
            }

            var assemblies = pluginLoader.Load(path);
            var manifest = Sandbox.ReadManifest(assemblies);

            _messageBus.Publish(new PluginLoadedMessage {Name = manifest.Name, Version = manifest.Version});
        }

        public void UnloadAll()
        {
            //
        }
    }
}
