using System;
using System.IO;

namespace Hadouken.Framework.IO
{
    public interface IFileSystem
    {
        void CreateDirectory(string path);

        string[] GetDirectoryEntries(string path);

        bool IsDirectory(string path);

        Stream OpenRead(string path);

        bool FileExists(string path);

        bool DirectoryExists(string path);

        DateTime? LastWriteTime(string path);
    }
}
