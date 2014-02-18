namespace Hadouken.Framework.IO
{
    public interface IDirectory
    {
        string FullPath { get; }

        string Name { get; }

        IDirectory[] Directories { get; }
        
        IFile[] Files { get; }

        bool Exists { get; }

        void Create();

        void CreateIfNotExists();

        void Delete(bool recursive = false);
    }
}
