using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hadouken.IO
{
    public class FileSystem : IFileSystem
    {
        public void CreateDirectory(string path)
        {
            Directory.CreateDirectory(path);
        }

        public string[] GetDirectoryEntries(string path)
        {
            return Directory.GetFileSystemEntries(path).ToArray();
        }

        public bool IsDirectory(string path)
        {
            if (!Directory.Exists(path))
                return false;

            return File.GetAttributes(path).HasFlag(FileAttributes.Directory);
        }

        public Stream OpenRead(string path)
        {
            return File.OpenRead(path);
        }

        public bool FileExists(string path)
        {
            return File.Exists(path);
        }
    }
}
