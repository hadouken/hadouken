using System.IO;
using Hadouken.Fx.IO;

namespace Hadouken.Plugins
{
    public class PackageInstaller : IPackageInstaller
    {
        private readonly string _pluginDirectory;
        private readonly IFileSystem _fileSystem;

        public PackageInstaller(string pluginDirectory, IFileSystem fileSystem)
        {
            _pluginDirectory = pluginDirectory;
            _fileSystem = fileSystem;
        }

        public void Install(IPackage package)
        {
            // Check if the package is already in the file system
            var directoryName = string.Format("{0}-{1}", package.Manifest.Name, package.Manifest.Version);
            var targetDirectoryPath = Path.Combine(_pluginDirectory, directoryName);
            var targetDirectory = _fileSystem.GetDirectory(targetDirectoryPath);

            if (targetDirectory.Exists)
            {
                throw new InstallPackageException("Package already exists.");
            }

            targetDirectory.Create();

            // Write all package files to the directory
            foreach (var file in package.Files)
            {
                var targetFilePath = Path.Combine(targetDirectoryPath, file.FullPath);
                var targetFile = _fileSystem.GetFile(targetFilePath);

                if (targetFile.Exists)
                {
                    continue;
                }

                using (var sourceStream = file.OpenRead())
                using (var targetStream = targetFile.OpenWrite())
                {
                    sourceStream.CopyTo(targetStream);
                }
            }
        }
    }
}