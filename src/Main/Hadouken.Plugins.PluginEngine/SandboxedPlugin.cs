using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Hadouken.Reflection;
using Hadouken.DI;

namespace Hadouken.Plugins.PluginEngine
{
    public class SandboxedPlugin : MarshalByRefObject
    {
        private IPlugin _plugin;
        private string _pluginName;
        private Version _pluginVersion;

        public void Load(ProxyResolver dependencyResolver)
        {
            if (_plugin != null)
                return;

            // Find the plugin type
            var info = (from asm in AppDomain.CurrentDomain.GetAssemblies()
                        from t in asm.GetTypes()
                        where t.IsClass && !t.IsAbstract
                        where t.HasAttribute<PluginAttribute>()
                        let a = t.GetAttribute<PluginAttribute>()
                        select new
                            {
                                Type = t,
                                a.Name,
                                a.Version
                            }).FirstOrDefault();

            if (info == null)
                return;

            _pluginName = info.Name;
            _pluginVersion = info.Version;

            var type = info.Type;
            var constructor = type.GetConstructors().First();
            var parameters = constructor.GetParameters().Select(cparam => dependencyResolver.Get(cparam.ParameterType)).ToArray();

            _plugin = (IPlugin)constructor.Invoke(parameters);
            _plugin.Load();
        }

        public void Unload()
        {
            _plugin.Unload();
        }

        public byte[] GetResource(string name)
        {
            throw new NotImplementedException();
        }

        public string Name
        {
            get { return _pluginName; }
        }

        public Version Version
        {
            get { return _pluginVersion; }
        }

        internal void LoadAssemblies(IEnumerable<byte[]> assemblies)
        {
            foreach (var assembly in (assemblies as byte[][] ?? assemblies.ToArray()))
            {
                AppDomain.CurrentDomain.Load(assembly);
            }
        }
    }
}
