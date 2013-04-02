using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Castle.DynamicProxy;
using Hadouken.DI;

namespace Hadouken.Plugins.PluginEngine
{
    public class SandboxedPluginManager : IPluginManager
    {
        private readonly SandboxedPlugin _plugin;

        public SandboxedPluginManager(SandboxedPlugin plugin)
        {
            _plugin = plugin;
        }

        public void Load()
        {
            _plugin.Load(new ProxyResolver(Kernel.Resolver));
        }

        public void Unload()
        {
            _plugin.Unload();
        }

        public byte[] GetResource(string name)
        {
            return _plugin.GetResource(name);
        }

        public string Name
        {
            get { return _plugin.Name; }
        }

        public Version Version
        {
            get { return _plugin.Version; }
        }
    }
}
