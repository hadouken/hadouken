using System.IO;

namespace Hadouken.Fx.IO
{
    public interface IFile
    {
        bool Exists { get; }

        string Name { get; }

        string FullPath { get; }

        Stream OpenRead();

        Stream OpenWrite();

        void Delete();
    }
}
