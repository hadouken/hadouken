using System.IO;

namespace Hadouken.Fx.IO
{
    public class File : IFile
    {
        private readonly FileInfo _fileInfo;

        public File(FileInfo fileInfo)
        {
            _fileInfo = fileInfo;
        }

        public bool Exists
        {
            get { return _fileInfo.Exists; }
        }

        public string Name
        {
            get { return _fileInfo.Name; }
        }

        public string FullPath
        {
            get { return _fileInfo.FullName; }
        }

        public Stream OpenRead()
        {
            return _fileInfo.OpenRead();
        }

        public Stream OpenWrite()
        {
            return _fileInfo.OpenWrite();
        }

        public void Delete()
        {
            _fileInfo.Delete();
        }
    }
}
