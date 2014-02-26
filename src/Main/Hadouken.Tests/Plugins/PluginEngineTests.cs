using System;
using System.Linq;
using Hadouken.Configuration;
using Hadouken.Fx.IO;
using Hadouken.Plugins;
using Hadouken.Plugins.Isolation;
using Hadouken.Plugins.Metadata;
using Hadouken.Plugins.Repository;
using Moq;
using NUnit.Framework;

namespace Hadouken.Tests.Plugins
{
    [TestFixture]
    public class PluginEngineTests
    {
        [Test]
        public void Scan_WithNoPlugins_GivesEmptyResult()
        {
            // Given
            var scanner = CreatePluginScanner();
            var engine = new PluginEngine(new[] {scanner},
                new Mock<IPackageInstaller>().Object,
                new Mock<IPackageDownloader>().Object);

            // When
            engine.Scan();

            // Then
            Assert.AreEqual(0, engine.GetAll().Count());
        }

        [Test]
        public void Scan_WithPlugins_GivesCorrectResult()
        {
            // Given
            var manager1 = CreatePluginManager("a", "1.0");
            var manager2 = CreatePluginManager("b", "1.0");
            var scanner = CreatePluginScanner(manager1, manager2);
            var engine = new PluginEngine(new[] { scanner },
                new Mock<IPackageInstaller>().Object,
                new Mock<IPackageDownloader>().Object);

            // When
            engine.Scan();

            // Then
            Assert.AreEqual(2, engine.GetAll().Count());
        }

        [Test]
        public void Scan_WithPluginsHavingMissingDependencies_GivesCorrectResult()
        {
            // Given
            var manager1 = CreatePluginManager("a", "1.0");
            var manager2 = CreatePluginManager("b", "1.0", new Dependency { Name = "c" });
            var scanner = CreatePluginScanner(manager1, manager2);
            var engine = new PluginEngine(new[] { scanner },
                new Mock<IPackageInstaller>().Object,
                new Mock<IPackageDownloader>().Object);

            // When
            engine.Scan();

            // Then
            Assert.AreEqual(2, engine.GetAll().Count());
        }

        [Test]
        public void Load_WithPluginHavingMissingDependency_DoesNotLoad()
        {
            // Given
            var manager = CreatePluginManager("a", "1.0", new Dependency { Name = "test-missing-dependency" });
            var scanner = CreatePluginScanner(manager);
            var engine = new PluginEngine(new[] { scanner },
                new Mock<IPackageInstaller>().Object,
                new Mock<IPackageDownloader>().Object);

            // When
            engine.Scan();
            engine.Load("a");

            // Then
            Assert.AreEqual(PluginState.Unloaded, engine.Get("a").State);
        }

        [Test]
        public void Load_WithValidPlugin_Loads()
        {
            // Given
            var manager = CreatePluginManager("a", "1.0");
            var scanner = CreatePluginScanner(manager);
            var engine = new PluginEngine(new[] { scanner },
                new Mock<IPackageInstaller>().Object,
                new Mock<IPackageDownloader>().Object);

            // When
            engine.Scan();
            engine.Load("a");

            // Then
            Assert.AreEqual(PluginState.Loaded, engine.Get("a").State);
        }

        [Test]
        public void Load_WithPluginHavingComplexDependencies_LoadsAllDependencies()
        {
            // Given
            var manager1 = CreatePluginManager("a", "1.0");
            var manager2 = CreatePluginManager("b", "1.0", new Dependency {Name = "a"});
            var manager3 = CreatePluginManager("c", "1.0", new Dependency {Name = "b"}, new Dependency {Name = "a"});
            var manager4 = CreatePluginManager("d", "1.0", new Dependency {Name = "c"});
            var scanner = CreatePluginScanner(manager1, manager2, manager3, manager4);
            var engine = new PluginEngine(new[] { scanner },
                new Mock<IPackageInstaller>().Object,
                new Mock<IPackageDownloader>().Object);

            // When
            engine.Scan();
            engine.Load("d");

            // Then
            Assert.AreEqual(PluginState.Loaded, engine.Get("a").State);
            Assert.AreEqual(PluginState.Loaded, engine.Get("b").State);
            Assert.AreEqual(PluginState.Loaded, engine.Get("c").State);
            Assert.AreEqual(PluginState.Loaded, engine.Get("d").State);
        }

        [Test]
        public void Load_WithPluginHavingComplexDependenciesAndOneIsMissing_DoesNotLoadAnyPlugin()
        {
            // Given
            var manager1 = CreatePluginManager("a", "1.0", new Dependency {Name = "missing"});
            var manager2 = CreatePluginManager("b", "1.0", new Dependency {Name = "a"});
            var manager3 = CreatePluginManager("c", "1.0", new Dependency {Name = "b"}, new Dependency {Name = "a"});
            var manager4 = CreatePluginManager("d", "1.0", new Dependency {Name = "c"});
            var scanner = CreatePluginScanner(manager1, manager2, manager3, manager4);
            var engine = new PluginEngine(new[] { scanner },
                new Mock<IPackageInstaller>().Object,
                new Mock<IPackageDownloader>().Object);

            // When
            engine.Scan();
            engine.Load("d");

            // Then
            Assert.AreEqual(PluginState.Unloaded, engine.Get("a").State);
            Assert.AreEqual(PluginState.Unloaded, engine.Get("b").State);
            Assert.AreEqual(PluginState.Unloaded, engine.Get("c").State);
            Assert.AreEqual(PluginState.Unloaded, engine.Get("d").State);
        }

        [Test]
        public void Load_WithPluginHavingMissingDependency_DownloadsAndInstallsMissingDependency()
        {
            // Given
            var manager = CreatePluginManager("a", "1.0", new Dependency {Name = "missing"});
            var scanner = CreatePluginScannerMock(manager);
            var installer = new Mock<IPackageInstaller>();
            installer.Setup(i => i.Install(It.IsAny<IPackage>()))
                .Callback(
                    () => scanner.Setup(s => s.Scan()).Returns(new[] { manager, CreatePluginManager("missing", "1.0") }));
            var downloader = new Mock<IPackageDownloader>();
            downloader.Setup(d => d.Download("missing")).Returns(CreatePackage("missing", "1.0"));
            var engine = new PluginEngine(new[]{scanner.Object}, installer.Object, downloader.Object);
            
            // When
            engine.Scan();
            engine.Load("a");

            // Then
            downloader.Verify(d => d.Download("missing"), Times.Once());
            installer.Verify(i => i.Install(It.IsAny<IPackage>()), Times.Once());

            Assert.AreEqual(PluginState.Loaded, engine.Get("missing").State);
            Assert.AreEqual(PluginState.Loaded, engine.Get("a").State);
        }

        [Test]
        public void Load_WithDownloadedDependencyHavingMissingDependency_DoesNotLoadAnything()
        {
            // Given
            var manager = CreatePluginManager("a", "1.0", new Dependency { Name = "missing" });
            var scanner = CreatePluginScannerMock(manager);
            var installer = new Mock<IPackageInstaller>();
            installer.Setup(i => i.Install(It.IsAny<IPackage>()))
                .Callback(
                    () =>
                        scanner.Setup(s => s.Scan())
                            .Returns(new[]
                            {manager, CreatePluginManager("missing", "1.0", new Dependency {Name = "another-missing"})}));
            var downloader = new Mock<IPackageDownloader>();
            downloader.Setup(d => d.Download("missing")).Returns(CreatePackage("missing", "1.0"));
            var engine = new PluginEngine(new[] { scanner.Object }, installer.Object, downloader.Object);

            // When
            engine.Scan();
            engine.Load("a");

            // Then
            downloader.Verify(d => d.Download("missing"), Times.Once());
            downloader.Verify(d => d.Download("another-missing"), Times.Once());
            installer.Verify(i => i.Install(It.IsAny<IPackage>()), Times.Once());

            Assert.AreEqual(PluginState.Unloaded, engine.Get("missing").State);
            Assert.AreEqual(PluginState.Unloaded, engine.Get("a").State);
        }

        private IPluginManager CreatePluginManager(string name, string version, params Dependency[] dependencies)
        {
            var envMock = new Mock<IIsolatedEnvironment>();
            var cfg = CreateConfiguration();
            var dirMock = new Mock<IDirectory>();
            var manifestMock = new Mock<IManifest>();
            manifestMock.SetupGet(m => m.Name).Returns(name);
            manifestMock.SetupGet(m => m.Version).Returns(version);
            manifestMock.SetupGet(m => m.Dependencies).Returns(dependencies);

            return new PluginManager(cfg, dirMock.Object, envMock.Object, manifestMock.Object);
        }

        private IPluginScanner CreatePluginScanner(params IPluginManager[] managers)
        {
            var mock = new Mock<IPluginScanner>();
            mock.Setup(m => m.Scan()).Returns(managers);
            return mock.Object;
        }

        private Mock<IPluginScanner> CreatePluginScannerMock(params IPluginManager[] managers)
        {
            var mock = new Mock<IPluginScanner>();
            mock.Setup(m => m.Scan()).Returns(managers);
            return mock;
        }

        private IPackage CreatePackage(string name, string version, params Dependency[] dependencies)
        {
            var pkg = new Mock<IPackage>();
            var man = new Mock<IManifest>();
            man.SetupGet(m => m.Dependencies).Returns(dependencies);
            man.SetupGet(m => m.Name).Returns(name);
            man.SetupGet(m => m.Version).Returns(version);

            pkg.SetupGet(p => p.Manifest).Returns(man.Object);

            return pkg.Object;
        }

        private IConfiguration CreateConfiguration()
        {
            var cfg = new Mock<IConfiguration>();
            cfg.SetupGet(c => c.ApplicationDataPath).Returns("test");
            return cfg.Object;
        }
    }
}
