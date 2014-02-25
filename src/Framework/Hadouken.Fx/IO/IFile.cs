using System.IO;

namespace Hadouken.Fx.IO
{
    public interface IFile
    {
        bool Exists { get; }

        string FileName { get; }

        Stream OpenRead();

        Stream OpenWrite();
    }
}
