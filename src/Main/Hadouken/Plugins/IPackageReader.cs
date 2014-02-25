using System.IO;

namespace Hadouken.Plugins
{
    public interface IPackageReader
    {
        /// <summary>
        /// Reads a package from the specified stream.
        /// </summary>
        /// <param name="stream">The stream which should be a valid plugin package.</param>
        /// <returns>An IPackage instance or null if the stream was invalid.</returns>
        IPackage Read(Stream stream);
    }
}
