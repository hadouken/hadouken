using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hadouken.Plugins;
using Hadouken.Data.Models;
using Hadouken.Data;
using System.Reflection;

namespace Hadouken.Impl.Plugins
{
    public class DefaultPluginManager : IPluginManager
    {
        private PluginInfo _info;
        private IPlugin _instance;

        private IMigratorRunner _runner;
        private IPluginLoader[] _loaders;

        internal DefaultPluginManager(PluginInfo info, IMigratorRunner runner, IPluginLoader[] loaders)
        {
            _info = info;

            _runner = runner;
            _loaders = loaders;
        }

        internal void Initialize()
        {
            var loader = (from l in _loaders
                          where l.CanLoad(_info.Path)
                          select l).First();

            if (loader != null)
            {
                Type t = loader.Load(_info.Path).First();

                if (t != null)
                {
                    // create IPlugin instance
                    _instance = CreatePluginFromType(t);

                    if (_instance == null)
                        throw new Exception("Could not load plugin");
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
        }

        public void Uninstall()
        {
            // run migrator down
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
