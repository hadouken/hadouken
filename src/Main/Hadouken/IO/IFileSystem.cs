using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Hadouken.IO
{
    public interface IFileSystem : IComponent
    {
        bool IsDirectory(string path);

        Stream OpenRead(string path);

        byte[] ReadAllBytes(string path);

        bool FileExists(string path);
        bool DirectoryExists(string path);

        FileSystemInfo[] GetFileSystemInfos(string path);
        string[] GetFiles(string path);

        void DeleteDirectory(string path);
        void CreateDirectory(string path);

        string[] GetDirectories(string path);
    }
}
