namespace Hadouken.Fx.IO
{
    public interface IFileSystem
    {
        IDirectory GetDirectory(string path);

        IFile GetFile(string path);
    }
}
