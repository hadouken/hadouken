using System;
using System.Collections.Generic;
using System.Linq;
using Hadouken.IO;
using Hadouken.Reflection;

namespace Hadouken.Plugins.PluginEngine.Loaders
{
    public class AssemblyPluginLoader : IPluginLoader
    {
        private IFileSystem _fileSystem;

        public AssemblyPluginLoader(IFileSystem fs)
        {
            _fileSystem = fs;
        }

        public bool CanLoad(string path)
        {
            return path.EndsWith(".dll");
        }

        public IEnumerable<byte[]> Load(string path)
        {
            return new [] { _fileSystem.ReadAllBytes(path) };
        }
    }
}
