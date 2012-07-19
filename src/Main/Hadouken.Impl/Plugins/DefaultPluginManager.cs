using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hadouken.Plugins;
using Hadouken.Data.Models;
using Hadouken.Data;
using System.Reflection;
using NLog;
using Hadouken.Messaging;
using Hadouken.Messages;

namespace Hadouken.Impl.Plugins
{
    public class DefaultPluginManager : IPluginManager
    {
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        private PluginInfo _info;
        private IPlugin _instance;

        private IMessageBus _mbus;
        private IMigrationRunner _runner;
        private IPluginLoader[] _loaders;

        internal DefaultPluginManager(PluginInfo info, IMessageBus mbus, IMigrationRunner runner, IPluginLoader[] loaders)
        {
            _info = info;

            _mbus = mbus;
            _runner = runner;
            _loaders = loaders;
        }

        internal void Initialize()
        {
            _logger.Debug("Initializing plugin manager for {0}", _info.Name);

            var loader = (from l in _loaders
                          where l.CanLoad(_info.Path)
                          select l).First();

            if (loader != null)
            {
                Type t = loader.Load(_info.Path).First();

                if (t != null)
                {
                    // register types in plugin assembly
                    Kernel.Register(t.Assembly);

                    // send IPluginLoading
                    _mbus.Send<IPluginLoading>(msg =>
                    {
                        msg.PluginName = _info.Name;
                        msg.PluginVersion = _info.Version;
                        msg.PluginType = t;
                    }).Wait();

                    // create IPlugin instance
                    _instance = CreatePluginFromType(t);

                    if (_instance == null)
                        throw new Exception("Could not load plugin");

                    _logger.Info("Plugin {0} loaded", _info.Name);
                }
            }
        }

        private IPlugin CreatePluginFromType(Type t)
        {
            ConstructorInfo ctor = t.GetConstructors().First();
            List<object> args = new List<object>();

            foreach (var parameter in ctor.GetParameters())
            {
                object instance = Kernel.Get(parameter.ParameterType);

                if (instance == null)
                    throw new ArgumentException(parameter.Name);

                args.Add(instance);
            }

            return ctor.Invoke(args.ToArray()) as IPlugin;
        }

        public void Load()
        {
            _instance.Load();
        }

        public void Unload()
        {
            _instance.Unload();
        }

        public void Install()
        {
            // run migrator up
            _runner.Up(_instance.GetType().Assembly);
        }

        public void Uninstall()
        {
            // run migrator down
            _runner.Down(_instance.GetType().Assembly);
        }

        public string Name
        {
            get { return _info.Name; }
        }

        public Version Version
        {
            get { return _info.Version; }
        }
    }
}
