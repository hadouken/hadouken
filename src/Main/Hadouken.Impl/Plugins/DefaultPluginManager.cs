using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hadouken.Plugins;
using Hadouken.Data.Models;
using Hadouken.Data;
using Hadouken.Reflection;
using System.Reflection;
using NLog;
using Hadouken.Messaging;
using Hadouken.Messages;

namespace Hadouken.Impl.Plugins
{
    public class DefaultPluginManager : IPluginManager
    {
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        private Type _pluginType;
        private IPlugin _instance;

        private IMessageBus _mbus;
        private IMigrationRunner _runner;

        internal DefaultPluginManager(Type pluginType, IMessageBus mbus, IMigrationRunner runner)
        {
            _pluginType = pluginType;

            _mbus = mbus;
            _runner = runner;
        }

        internal void Initialize()
        {
            PluginAttribute attr = _pluginType.GetAttribute<PluginAttribute>();

            _logger.Info("Loading plugin {0} [version {1}]", attr.Name, attr.Version);

            // register types in plugin assembly
            Kernel.Register(_pluginType.Assembly);

            // send IPluginLoading
            _mbus.Send<IPluginLoading>(msg =>
            {
                msg.PluginName = attr.Name;
                msg.PluginVersion = attr.Version;
                msg.PluginType = _pluginType;
            }).Wait();

            // create IPlugin instance
            _instance = CreatePluginFromType(_pluginType);

            if (_instance == null)
                throw new Exception("Could not load plugin");

            _mbus.Send<IPluginLoaded>(m => m.Manager = this).Wait();
        }

        private IPlugin CreatePluginFromType(Type t)
        {
            ConstructorInfo ctor = t.GetConstructors().First();
            List<object> args = new List<object>();

            foreach (var parameter in ctor.GetParameters())
            {
                object instance = Kernel.Resolver.Get(parameter.ParameterType);

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
            get { return _pluginType.GetAttribute<PluginAttribute>().Name; }
        }

        public Version Version
        {
            get { return _pluginType.GetAttribute<PluginAttribute>().Version; }
        }

        public string ResourceBase
        {
            get { return _pluginType.GetAttribute<PluginAttribute>().ResourceBase; }
        }

        public Type PluginType
        {
            get { return _pluginType; }
        }
    }
}
