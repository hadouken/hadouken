using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

using Hadouken.Plugins;
using Hadouken.Data;
using Hadouken.Data.Models;
using Hadouken.IO;
using Hadouken.Reflection;
using System.IO;
using System.Configuration;
using Hadouken.Configuration;
using NLog;

namespace Hadouken.Impl.Plugins
{
    public class DefaultPluginFactory : IPluginFactory
    {
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        private IDataRepository _data;
        private IFileSystem _fs;
        private IMigratorRunner _runner;
        private IPluginLoader[] _loaders;

        public DefaultPluginFactory(IFileSystem fs, IDataRepository data, IMigratorRunner migratorRunner, IPluginLoader[] loaders)
        {
            _fs = fs;
            _data = data;
            _runner = migratorRunner;
            _loaders = loaders;
        }

        public void ScanForChanges()
        {
            _logger.Info("Scanning for new plugins/removing deleted plugins");

            RemoveDeletedPlugins();
            AddNewPlugins();
        }

        public void Load(string name)
        {
            var pi = _data.Single<PluginInfo>(p => p.Name == name);

            if (pi != null)
                LoadFromPluginInfo(pi);
        }

        public void LoadAll()
        {
            var piList = _data.List<PluginInfo>();

            foreach (var pi in piList)
                LoadFromPluginInfo(pi);
        }

        private void LoadFromPluginInfo(PluginInfo info)
        {
            foreach (var loader in _loaders)
            {
                if (loader.CanLoad(info.Path))
                {
                    IEnumerable<Type> pluginTypes = loader.Load(info.Path);

                    foreach (var type in pluginTypes)
                        RegisterAndPossiblyLoad(info, type);

                    break;
                }
            }
        }

        private void RegisterAndPossiblyLoad(PluginInfo info, Type pluginType)
        {
            // run migrations from plugin assembly
            _runner.Run(pluginType.Assembly);

            Kernel.Register(pluginType.Assembly);
            Kernel.Bind(typeof(IPlugin), pluginType, ComponentLifestyle.Singleton, info.Name);

            IPlugin plugin = Kernel.Get<IPlugin>(info.Name);
            plugin.Load();
        }

        public void Unload(string name)
        {
            IPlugin plugin = Kernel.TryGet<IPlugin>(name);

            if (plugin != null)
                plugin.Unload();
        }

        public void UnloadAll()
        {
            var plugins = Kernel.GetAll<IPlugin>();

            foreach (var plugin in plugins)
                plugin.Unload();
        }

        private void RemoveDeletedPlugins()
        {
            var pluginInfoList = _data.List<PluginInfo>();

            foreach (var pluginInfo in pluginInfoList)
            {
                if (!_fs.FileExists(pluginInfo.Path) || !_fs.DirectoryExists(pluginInfo.Path))
                {
                    _data.Delete(pluginInfo);
                }
            }
        }

        private void AddNewPlugins()
        {
            string pluginPath = HdknConfig.GetPath("Paths.Plugins");

            _logger.Info("Scanning for new plugins in path {0}", pluginPath);

            foreach (FileSystemInfo info in _fs.GetFileSystemInfos(pluginPath))
            {
                if (_data.Single<PluginInfo>(p => p.Path == info.FullName) == null)
                {
                    foreach (var loader in _loaders)
                    {
                        if (loader.CanLoad(info.FullName))
                        {
                            var types = loader.Load(info.FullName);

                            foreach (var type in types)
                            {
                                _logger.Info("Found new plugin {0}", type.FullName);

                                PluginInfo pi = new PluginInfo();
                                pi.Name = type.Assembly.GetAttribute<AssemblyTitleAttribute>().Title;
                                pi.Version = type.Assembly.GetName().Version;
                                pi.Path = info.FullName;
                                pi.State = PluginState.Uninstalled;

                                _data.Save(pi);
                            }

                            break;
                        }
                    }
                }
            }
        }
    }
}
