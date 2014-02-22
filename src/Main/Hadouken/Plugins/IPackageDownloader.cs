namespace Hadouken.Plugins
{
    public interface IPackageDownloader
    {
        IPackage Download(string packageId);
    }
}
