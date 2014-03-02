namespace Hadouken.Plugins.Repository
{
    public interface IPackageDownloader
    {
        IPackage Download(string packageId);
    }
}
