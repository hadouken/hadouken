using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Hadouken.IO
{
    public interface IFileSystem
    {
        bool IsDirectory(string path);

        Stream OpenRead(string path);

        byte[] ReadAllBytes(string path);
        void WriteAllBytes(string path, byte[] bytes);

        bool FileExists(string path);
        bool DirectoryExists(string path);

        FileSystemInfo[] GetFileSystemInfos(string path);

        string[] GetFiles(string path);
        string[] GetFiles(string path, string pattern);
        string[] GetFiles(string path, string pattern, SearchOption option);

        void DeleteFile(string path);
        void DeleteDirectory(string path);
        void CreateDirectory(string path);

        string[] GetDirectories(string path);

        /// <summary>
        /// Calculates remaining disk space on the specified drive.
        /// </summary>
        /// <param name="dir">Path on the drive</param>
        /// <returns>The remaining disk space in bytes</returns>
        long RemainingDiskSpace(string dir);

        /// <summary>
        /// Deletes all files and folders in the path given.
        /// </summary>
        /// <param name="path">The path to clean.</param>
        void EmptyDirectory(string path);
    }
}
