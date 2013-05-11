using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hadouken.Plugins;
using Hadouken.IO;
using Hadouken.Reflection;

namespace Hadouken.Impl.Plugins
{
    [Component]
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

        public IEnumerable<byte[]> Load(string path)
        {
            var asm = _fs.ReadAllBytes(path);

            return new[] {asm};
        }
    }
}
