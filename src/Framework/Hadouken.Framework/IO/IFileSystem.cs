using System;
using System.IO;

namespace Hadouken.Framework.IO
{
    public interface IFileSystem
    {
        string RootPath { get; }

        IDirectory GetDirectory(string path);

        IFile GetFile(string path);
    }
}
