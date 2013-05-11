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

        public IEnumerable<byte[]> Load(string path)
        {
            var assemblies = (from f in _fs.GetFiles(path)
                              where f.EndsWith(".dll")
                              select _fs.ReadAllBytes(f));

            return assemblies;
        }
    }
}
