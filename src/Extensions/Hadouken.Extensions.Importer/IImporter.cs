namespace Hadouken.Extensions.Importer
{
    public interface IImporter
    {
        string Name { get; }

        void Import(string dataPath);
    }
}
