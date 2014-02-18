using System;
using System.IO;
using System.Linq;

namespace Hadouken.Framework.IO.Local
{
    public class LocalDirectory : IDirectory
    {
        private readonly DirectoryInfo _directoryInfo;

        public LocalDirectory(DirectoryInfo directoryInfo)
        {
            _directoryInfo = directoryInfo;
        }

        public string FullPath
        {
            get { return _directoryInfo.FullName; }
        }

        public string Name
        {
            get { return _directoryInfo.Name; }
        }

        public IDirectory[] Directories
        {
            get { throw new NotImplementedException(); }
        }

        public IFile[] Files
        {
            get
            {
                return
                    _directoryInfo.GetFileSystemInfos("*", SearchOption.AllDirectories)
                        .Where(f => File.Exists(f.FullName))
                        .Select(f => new LocalFile(new FileInfo(f.FullName)))
                        .ToArray();
            }
        }

        public bool Exists
        {
            get { return _directoryInfo.Exists; }
        }

        public void Create()
        {
            _directoryInfo.Create();
        }

        public void CreateIfNotExists()
        {
            if (!Exists)
                Create();
        }

        public void Delete(bool recursive = false)
        {
            _directoryInfo.Delete(recursive);
        }
    }
}
