using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hadouken.IO;
using System.IO;

namespace Hadouken.Impl.IO
{
    public class LocalFileSystem : IFileSystem
    {
        public bool IsDirectory(string path)
        {
            return (File.GetAttributes(path) & FileAttributes.Directory) == FileAttributes.Directory;
        }

        public Stream OpenRead(string path)
        {
            return File.OpenRead(path);
        }

        public byte[] ReadAllBytes(string path)
        {
            return File.ReadAllBytes(path);
        }

        public bool FileExists(string path)
        {
            return File.Exists(path);
        }

        public bool DirectoryExists(string path)
        {
            return Directory.Exists(path);
        }

        public FileSystemInfo[] GetFileSystemInfos(string path)
        {
            return new DirectoryInfo(path).GetFileSystemInfos();
        }

        public string[] GetFiles(string path)
        {
            return Directory.GetFiles(path);
        }
    }
}
