using System;
using System.IO;

namespace Hadouken.Framework.IO.Local
{
    public class LocalFile : IFile
    {
        private readonly FileInfo _fileInfo;

        public LocalFile(FileInfo fileInfo)
        {
            _fileInfo = fileInfo;
        }

        public string FullPath
        {
            get { return _fileInfo.FullName; }
        }

        public string Name
        {
            get { return _fileInfo.Name; }
        }

        public string Extension
        {
            get { return Path.GetExtension(_fileInfo.Name); }
        }

        public void Move(IDirectory targetDirectory)
        {
            throw new NotImplementedException();
        }

        public long Size
        {
            get { return _fileInfo.Length; }
        }

        public DateTime? LastWriteTime
        {
            get { return _fileInfo.LastWriteTime; }
        }

        public Stream OpenRead()
        {
            return _fileInfo.OpenRead();
        }

        public Stream OpenWrite()
        {
            return _fileInfo.OpenWrite();
        }
    }
}
