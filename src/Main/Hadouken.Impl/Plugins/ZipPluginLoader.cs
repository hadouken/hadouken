using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hadouken.Plugins;
using Hadouken.IO;
using System.Reflection;
using Ionic.Zip;
using System.IO;
using Hadouken.Reflection;

namespace Hadouken.Impl.Plugins
{
    public class ZipPluginLoader : IPluginLoader
    {
        private IFileSystem _fs;

        public ZipPluginLoader(IFileSystem fs)
        {
            _fs = fs;
        }

        public bool CanLoad(string path)
        {
            // TODO: check file header as well
            byte[] data = _fs.ReadAllBytes(path);

            int header = (data[0] << 24) | (data[1] << 16) | (data[2] << 8) | data[3];

            return (header == 0x04034b50 && !_fs.IsDirectory(path) && path.EndsWith(".zip"));
        }

        public IEnumerable<Type> Load(string path)
        {
            List<Assembly> assemblies = new List<Assembly>();

            using (ZipFile file = ZipFile.Read(_fs.OpenRead(path)))
            {
                foreach (ZipEntry entry in file.Entries.Where(e => e.FileName.EndsWith(".dll")))
                {
                    using (var ms = new MemoryStream())
                    {
                        entry.Extract(ms);
                        assemblies.Add(AppDomain.CurrentDomain.Load(ms.ToArray()));
                    }
                }
            }

            return from asm in assemblies
                   from type in asm.GetTypes()
                   where type.IsClass && !type.IsAbstract && type.HasAttribute<PluginAttribute>()
                   where typeof(IPlugin).IsAssignableFrom(type)
                   select type;
        }
    }
}
