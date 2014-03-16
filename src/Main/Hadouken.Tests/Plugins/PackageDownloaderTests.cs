using System;
using System.IO;
using Hadouken.Http;
using Hadouken.Http.Api;
using Hadouken.Http.Api.Models;
using Hadouken.Plugins;
using NSubstitute;
using Xunit;

namespace Hadouken.Tests.Plugins
{
    public class PackageDownloaderTests
    {
        public class TheConstructor
        {
            [Fact]
            public void Should_Throw_ArgumentNullException_If_PluginRepository_Is_Null()
            {
                // Given, When, Then
                Assert.Throws<ArgumentNullException>(
                    () =>
                        new PackageDownloader(null, Substitute.For<IApiConnection>(), Substitute.For<IPackageReader>()));
            }

            [Fact]
            public void Should_Throw_ArgumentNullException_If_ApiConnection_Is_Null()
            {
                // Given, When, Then
                Assert.Throws<ArgumentNullException>(
                    () =>
                        new PackageDownloader(Substitute.For<IPluginRepository>(), null, Substitute.For<IPackageReader>()));
            }

            [Fact]
            public void Should_Throw_ArgumentNullException_If_PackageReader_Is_Null()
            {
                // Given, When, Then
                Assert.Throws<ArgumentNullException>(
                    () =>
                        new PackageDownloader(Substitute.For<IPluginRepository>(), Substitute.For<IApiConnection>(), null));
            }
        }

        public class TheDownloadMethod
        {
            [Fact]
            public void Should_Call_PluginRepository_With_Correct_Id()
            {
                // Given
                var repository = Substitute.For<IPluginRepository>();
                var packageDownloader = new PackageDownloader(
                    repository,
                    Substitute.For<IApiConnection>(),
                    Substitute.For<IPackageReader>());

                // When
                packageDownloader.Download("PluginId");

                // Then
                repository.Received(1).GetById("PluginId");
            }

            [Fact]
            public void Should_Return_Null_If_Plugin_Does_Not_Exist()
            {
                // Given
                var repository = Substitute.For<IPluginRepository>();
                repository.GetById("PluginId").Returns(ci => null);
                var packageDownloader = new PackageDownloader(
                    repository,
                    Substitute.For<IApiConnection>(),
                    Substitute.For<IPackageReader>());

                // When
                var package = packageDownloader.Download("PluginId");

                // Then
                Assert.Null(package);
            }

            [Fact]
            public void Should_Return_Null_If_Plugin_Has_No_Release()
            {
                // Given
                var repository = Substitute.For<IPluginRepository>();
                repository.GetById("PluginId").Returns(ci => new Plugin());
                var packageDownloader = new PackageDownloader(
                    repository,
                    Substitute.For<IApiConnection>(),
                    Substitute.For<IPackageReader>());

                // When
                var package = packageDownloader.Download("PluginId");

                // Then
                Assert.Null(package);
            }

            [Fact]
            public void Should_Return_Null_If_Plugin_Release_Has_No_Data()
            {
                // Given
                var repository = Substitute.For<IPluginRepository>();
                repository.GetById("PluginId").Returns(ci => new Plugin() {Releases = new[] {new ReleaseItem()}});
                var api = Substitute.For<IApiConnection>();
                api.DownloadData(Arg.Any<Uri>()).Returns(ci => null);
                var packageDownloader = new PackageDownloader(
                    repository,
                    api,
                    Substitute.For<IPackageReader>());

                // When
                var package = packageDownloader.Download("PluginId");

                // Then
                api.Received(1).DownloadData(Arg.Any<Uri>());
                Assert.Null(package);
            }

            [Fact]
            public void Should_Call_PackageReader_After_Downloading_Plugin_Release_Data()
            {
                // Given
                var repository = Substitute.For<IPluginRepository>();
                repository.GetById("PluginId").Returns(ci => new Plugin() { Releases = new[] { new ReleaseItem() } });
                var api = Substitute.For<IApiConnection>();
                api.DownloadData(Arg.Any<Uri>()).Returns(new byte[] {1, 2, 3});
                var reader = Substitute.For<IPackageReader>();
                var packageDownloader = new PackageDownloader(
                    repository,
                    api,
                    reader);

                // When
                var package = packageDownloader.Download("PluginId");

                // Then
                reader.Received(1).Read(Arg.Any<MemoryStream>());
            }
        }
    }
}
