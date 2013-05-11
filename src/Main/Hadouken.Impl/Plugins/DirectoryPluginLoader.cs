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
    [Component]
    public class DirectoryPluginLoader : IPluginLoader
    {
        private readonly IFileSystem _fileSystem;

        public DirectoryPluginLoader(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        public bool CanLoad(string path)
        {
            return _fileSystem.IsDirectory(path);
        }

        public IEnumerable<byte[]> Load(string path)
        {
            var assemblies = (from f in _fileSystem.GetFiles(path)
                              where f.EndsWith(".dll")
                              select _fileSystem.ReadAllBytes(f));

            return assemblies;
        }
    }
}
