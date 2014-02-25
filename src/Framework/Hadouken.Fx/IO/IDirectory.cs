namespace Hadouken.Fx.IO
{
    public interface IDirectory
    {
        IDirectory[] Directories { get; }

        IFile[] Files { get; }

        string FullPath { get; } 
    }
}