namespace Hadouken.Plugins
{
    public interface IPackageInstaller
    {
        /// <summary>
        /// Writes the package to disk. Will not overwrite any existing package.
        /// </summary>
        /// <param name="package">The package to install.</param>
        void Install(IPackage package);
    }
}
