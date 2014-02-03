using System;
using Hadouken.Framework.IO;
using Hadouken.Plugins;
using Hadouken.Plugins.Metadata;
using Moq;
using NUnit.Framework;

namespace Hadouken.Tests.Plugins
{
    public class PackageTests
    {
        [Test]
        public void GetFile_Returns_File_Based_On_BaseUri()
        {
            var manifest = new Mock<IManifest>();
            var file = new Mock<IFile>();
            file.SetupGet(f => f.FullPath).Returns("c:/test/foo/subfolder/bar.txt");

            var package = new Package(manifest.Object, new[] {file.Object});
            package.BaseUri = new Uri("c:/test/foo/", UriKind.Absolute);

            var pkgFile = package.GetFile("subfolder/bar.txt");

            Assert.IsNotNull(pkgFile);
        }

        [Test]
        public void GetFile_Returns_File_When_BaseUri_Is_Null()
        {
            var manifest = new Mock<IManifest>();
            var file = new Mock<IFile>();
            file.SetupGet(f => f.FullPath).Returns("subfolder/bar.txt");

            var package = new Package(manifest.Object, new[] { file.Object });

            var pkgFile = package.GetFile("subfolder/bar.txt");

            Assert.IsNotNull(pkgFile);
        }
    }
}
