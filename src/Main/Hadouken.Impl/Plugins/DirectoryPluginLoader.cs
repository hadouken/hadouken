using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hadouken.Plugins;
using Hadouken.IO;
using System.Reflection;
using Hadouken.Reflection;

namespace Hadouken.Impl.Plugins
{
    public class DirectoryPluginLoader : IPluginLoader
    {
        private IFileSystem _fs;

        public DirectoryPluginLoader(IFileSystem fs)
        {
            _fs = fs;
        }

        public bool CanLoad(string path)
        {
            return _fs.IsDirectory(path);
        }

        public IEnumerable<Type> Load(string path)
        {
            var assemblies = (from f in _fs.GetFiles(path)
                              let data = _fs.ReadAllBytes(f)
                              select AppDomain.CurrentDomain.Load(data));

            return (from asm in assemblies
                    from type in asm.GetTypes()
                    where typeof(IPlugin).IsAssignableFrom(type)
                    where type.IsClass && !type.IsAbstract && type.HasAttribute<PluginAttribute>()
                    select type);
        }
    }
}
