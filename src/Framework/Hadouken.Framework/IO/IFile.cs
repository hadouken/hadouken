using System;
using System.IO;

namespace Hadouken.Framework.IO
{
    public interface IFile
    {
        string FullPath { get; }

        string Name { get; }

        string Extension { get; }

        void Move(IDirectory targetDirectory);

        long Size { get; }

        DateTime? LastWriteTime { get; }

        Stream OpenRead();

        Stream OpenWrite();
        
        void Delete();
    }
}
