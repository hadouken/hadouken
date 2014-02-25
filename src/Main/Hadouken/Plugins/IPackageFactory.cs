using System.IO;

namespace Hadouken.Plugins
{
    public interface IPackageFactory
    {
        /// <summary>
        /// Reads a package from the specified stream.
        /// </summary>
        /// <param name="stream">The stream which should be a valid plugin package.</param>
        /// <returns>An IPackage instance or null if the stream was invalid.</returns>
        IPackage ReadFrom(Stream stream);

        /// <summary>
        /// Saves the specified package to the plugin base directory. If a plugin with the same name and version is already
        /// present, this will return early and not overwrite anything.
        /// </summary>
        /// <param name="package">The package to save.</param>
        void Save(IPackage package);
    }
}
