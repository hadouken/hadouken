using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hadouken.Plugins;
using Hadouken.IO;
using Hadouken.Reflection;

namespace Hadouken.Impl.Plugins
{
    public class AssemblyPluginLoader : IPluginLoader
    {
        private IFileSystem _fs;

        public AssemblyPluginLoader(IFileSystem fs)
        {
            _fs = fs;
        }

        public bool CanLoad(string path)
        {
            return path.EndsWith(".dll");
        }

        public IEnumerable<Type> Load(string path)
        {
            var asm = AppDomain.CurrentDomain.Load(_fs.ReadAllBytes(path));

            return (from t in asm.GetTypes()
                    where typeof(IPlugin).IsAssignableFrom(t)
                    where t.IsClass && !t.IsAbstract && t.HasAttribute<PluginAttribute>()
                    select t);
        }
    }
}
