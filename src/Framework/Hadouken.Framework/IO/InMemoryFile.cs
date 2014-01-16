using System;
using System.IO;

namespace Hadouken.Framework.IO
{
    public class InMemoryFile : IFile
    {
        private readonly Func<Stream> _streamFactory;

        public InMemoryFile(Func<Stream> streamFactory)
        {
            _streamFactory = streamFactory;
        }

        public string FullPath { get; set; }

        public string Name { get; set; }

        public string Extension
        {
            get { return Path.GetExtension(Name); }
        }

        public void Move(IDirectory targetDirectory)
        {
            throw new NotImplementedException();
        }

        public long Size
        {
            get { return _streamFactory().Length; }
        }

        public DateTime? LastWriteTime
        {
            get { return null; }
        }

        public System.IO.Stream OpenRead()
        {
            return _streamFactory();
        }

        public System.IO.Stream OpenWrite()
        {
            throw new NotImplementedException();
        }

        public void Delete()
        {
        }
    }
}
