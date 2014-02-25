using System.IO;

namespace Hadouken.Fx.IO
{
    public class InMemoryFile : IFile
    {
        private readonly string _fileName;
        private readonly byte[] _data;

        public InMemoryFile(string fileName, byte[] data)
        {
            _fileName = fileName;
            _data = data;
        }

        public bool Exists { get; private set; }

        public string Name
        {
            get { return _fileName; }
        }

        public string FullPath
        {
            get { return _fileName; } 
        }

        public Stream OpenRead()
        {
            return new MemoryStream(_data);
        }

        public Stream OpenWrite()
        {
            throw new System.NotImplementedException();
        }
    }
}
