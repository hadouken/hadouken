using System.Collections.Generic;
using System.IO;
using System.Linq;
using Hadouken.Common.IO;
using Ionic.Zip;

namespace Hadouken.Plugins.PluginEngine.Loaders
{
    public class ZipPluginLoader : IPluginLoader
    {
        private readonly IFileSystem _fileSystem;

        public ZipPluginLoader(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        public bool CanLoad(string path)
        {
            if (_fileSystem.IsDirectory(path))
                return false;

            byte[] data = _fileSystem.ReadAllBytes(path);
            int header = (data[0] << 24) | (data[1] << 16) | (data[2] << 8) | data[3];

            return (header == 0x04034b50 && path.EndsWith(".zip"));
        }

        public IEnumerable<byte[]> Load(string path)
        {
            var data = new List<byte[]>();

            using (ZipFile file = ZipFile.Read(_fileSystem.OpenRead(path)))
            {
                foreach(var entry in file.Entries.Where(e => e.FileName.EndsWith(".dll")))
                {
                    using (var ms = new MemoryStream())
                    {
                        entry.Extract(ms);
                        data.Add(ms.ToArray());
                    }
                }
            }

            return data;
        }
    }
}
