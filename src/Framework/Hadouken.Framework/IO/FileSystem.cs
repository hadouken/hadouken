using System;
using System.IO;
using System.Linq;

namespace Hadouken.Framework.IO
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

        public bool DirectoryExists(string path)
        {
            return Directory.Exists(path);
        }

        public DateTime? LastWriteTime(string path)
        {
            if (!File.Exists(path))
                return null;

            var info = new FileInfo(path);
            return info.LastWriteTime;
        }
    }
}
