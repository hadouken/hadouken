using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hadouken.IO;
using System.IO;

namespace Hadouken.Impl.IO
{
    [Component]
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

        public void WriteAllBytes(string path, byte[] bytes)
        {
            File.WriteAllBytes(path, bytes);
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

        public string[] GetFiles(string path, string pattern)
        {
            return Directory.GetFiles(path, pattern);
        }

        public string[] GetFiles(string path, string pattern, SearchOption option)
        {
            return Directory.GetFiles(path, pattern, option);
        }

        public string[] GetDirectories(string path)
        {
            return Directory.GetDirectories(path);
        }

        public void CreateDirectory(string path)
        {
            Directory.CreateDirectory(path);
        }

        public void DeleteFile(string path)
        {
            File.Delete(path);
        }

        public void DeleteDirectory(string path)
        {
            Directory.Delete(path);
        }

        public void EmptyDirectory(string path)
        {
            var info = new DirectoryInfo(path);

            foreach(var file in info.GetFiles())
                file.Delete();

            foreach(var dir in info.GetDirectories())
                dir.Delete();
        }

        public long RemainingDiskSpace(string path)
        {
            string root = Path.GetPathRoot(path);

            if (String.IsNullOrEmpty(root))
                return -1;

            var drive = DriveInfo.GetDrives().FirstOrDefault(d => d.DriveType == DriveType.Fixed && d.IsReady && d.Name.StartsWith(root));

            return drive != null ? drive.TotalFreeSpace : -1;
        }
    }
}
