using System.IO;
using Hadouken.Configuration;
using Hadouken.Framework;
using Hadouken.Framework.IO;
using Hadouken.Framework.Rpc;
using Hadouken.Plugins;
using Hadouken.Plugins.Isolation;
using Hadouken.Plugins.Metadata;
using Moq;
using NUnit.Framework;

namespace Hadouken.Tests.Plugins
{
    public class PluginEngineTests
    {
        [Test]
        public void Get_WithNullAsPluginId_ReturnsNull()
        {
            // Given
            var engine = CreatePluginEngine();

            // When
            var result = engine.Get(null);

            // Then
            Assert.IsNull(result);
        }

        [Test]
        public void Get_WithNonExistentPluginId_ReturnsNull()
        {
            // Given
            var engine = CreatePluginEngine();

            // When
            var result = engine.Get("<test>");

            // Then
            Assert.IsNull(result);
        }

        [Test]
        public void Get_WithPluginIdHavingMixedCasing_ReturnsValidPluginManager()
        {
            // Given
            var package = CreatePackage("plugin", "1.0");
            var factory = CreatePackageFactory(package);
            var engine = CreatePluginEngine(packageFactory: factory);

            // When
            var result = engine.Get("pLuGiN");

            // Then
            Assert.IsNotNull(result);
        }

        [Test]
        public void Load_WithValidPluginId_LoadsPlugin()
        {
            // Given
            var package = CreatePackage("plugin", "1.0");
            var factory = CreatePackageFactory(package);
            var engine = CreatePluginEngine(packageFactory: factory);

            // When
            engine.Load("plugin");

            // Then
            Assert.AreEqual(PluginState.Loaded, engine.Get("plugin").State);
        }

        [Test]
        public void Load_WithDependencies1_LoadsCorrectly()
        {
            // Given
            var pkgA = CreatePackage("a", "1.0");
            var pkgB = CreatePackage("b", "1.0", new Dependency {Name = "a"});
            var pkgC = CreatePackage("c", "1.0", new Dependency {Name = "a"});
            var factory = CreatePackageFactory(pkgA, pkgB, pkgC);
            var engine = CreatePluginEngine(packageFactory: factory);

            // When
            engine.Load("c");

            // Then
            Assert.AreEqual(PluginState.Loaded, engine.Get("a").State);
            Assert.AreEqual(PluginState.Unloaded, engine.Get("b").State);
            Assert.AreEqual(PluginState.Loaded, engine.Get("c").State);
        }

        [Test]
        public void Load_WithDependencies2_LoadsCorrectly()
        {
            // Given
            var pkgA = CreatePackage("a", "1.0");
            var pkgB = CreatePackage("b", "1.0", new Dependency { Name = "a" });
            var pkgC = CreatePackage("c", "1.0", new Dependency { Name = "b" });
            var factory = CreatePackageFactory(pkgA, pkgB, pkgC);
            var engine = CreatePluginEngine(packageFactory: factory);

            // When
            engine.Load("c");

            // Then
            Assert.AreEqual(PluginState.Loaded, engine.Get("a").State);
            Assert.AreEqual(PluginState.Loaded, engine.Get("b").State);
            Assert.AreEqual(PluginState.Loaded, engine.Get("c").State);
        }

        [Test]
        public void Load_WithDependencies3_LoadsCorrectly()
        {
            // Given
            var pkgA = CreatePackage("a", "1.0");
            var pkgB = CreatePackage("b", "1.0", new Dependency { Name = "a" });
            var pkgC = CreatePackage("c", "1.0", new Dependency { Name = "a" });
            var pkgD = CreatePackage("d", "1.0", new Dependency {Name = "b"}, new Dependency {Name = "c"});
            var factory = CreatePackageFactory(pkgA, pkgB, pkgC, pkgD);
            var engine = CreatePluginEngine(packageFactory: factory);

            // When
            engine.Load("d");

            // Then
            Assert.AreEqual(PluginState.Loaded, engine.Get("a").State);
            Assert.AreEqual(PluginState.Loaded, engine.Get("b").State);
            Assert.AreEqual(PluginState.Loaded, engine.Get("c").State);
            Assert.AreEqual(PluginState.Loaded, engine.Get("d").State);
        }

        [Test]
        public void Unload_WithDependencies1_UnloadsCorrectly()
        {
            // Given
            var pkgA = CreatePackage("a", "1.0");
            var pkgB = CreatePackage("b", "1.0", new Dependency { Name = "a" });
            var pkgC = CreatePackage("c", "1.0", new Dependency { Name = "a" });
            var factory = CreatePackageFactory(pkgA, pkgB, pkgC);
            var engine = CreatePluginEngine(packageFactory: factory);
            engine.LoadAll();

            // When
            engine.Unload("a");

            // Then
            Assert.AreEqual(PluginState.Unloaded, engine.Get("a").State);
            Assert.AreEqual(PluginState.Unloaded, engine.Get("b").State);
            Assert.AreEqual(PluginState.Unloaded, engine.Get("c").State);
        }

        [Test]
        public void Unload_WithDependencies2_UnloadsCorrectly()
        {
            // Given
            var pkgA = CreatePackage("a", "1.0");
            var pkgB = CreatePackage("b", "1.0");
            var pkgC = CreatePackage("c", "1.0", new Dependency { Name = "a" });
            var factory = CreatePackageFactory(pkgA, pkgB, pkgC);
            var engine = CreatePluginEngine(packageFactory: factory);
            engine.LoadAll();

            // When
            engine.Unload("a");

            // Then
            Assert.AreEqual(PluginState.Unloaded, engine.Get("a").State);
            Assert.AreEqual(PluginState.Loaded, engine.Get("b").State);
            Assert.AreEqual(PluginState.Unloaded, engine.Get("c").State);
        }

        [Test]
        public void Unload_WithDependencies3_UnloadsCorrectly()
        {
            // Given
            var pkgA = CreatePackage("a", "1.0");
            var pkgB = CreatePackage("b", "1.0", new Dependency { Name = "a" });
            var pkgC = CreatePackage("c", "1.0", new Dependency { Name = "a" });
            var pkgD = CreatePackage("d", "1.0", new Dependency { Name = "b" }, new Dependency { Name = "c" });
            var factory = CreatePackageFactory(pkgA, pkgB, pkgC, pkgD);
            var engine = CreatePluginEngine(packageFactory: factory);
            engine.LoadAll();

            // When
            engine.Unload("a");

            // Then
            Assert.AreEqual(PluginState.Unloaded, engine.Get("a").State);
            Assert.AreEqual(PluginState.Unloaded, engine.Get("b").State);
            Assert.AreEqual(PluginState.Unloaded, engine.Get("c").State);
            Assert.AreEqual(PluginState.Unloaded, engine.Get("d").State);
        }

        [Test]
        public void InstallOrUpgrade_WithNewPlugin_LoadsCorrectly()
        {
            // Given
            var pkg = CreatePackage("a", "1.0");
            var engine = CreatePluginEngine();

            // When
            engine.LoadAll();
            engine.InstallOrUpgrade(pkg);

            // Then
            Assert.AreEqual(PluginState.Loaded, engine.Get("a").State);
        }

        [Test]
        public void InstallOrUpgrade_WithHigherVersionOfExistingPlugin_LoadsCorrectly()
        {
            // Given
            var pkg = CreatePackage("a", "1.0");
            var factory = CreatePackageFactory(pkg);
            var engine = CreatePluginEngine(packageFactory: factory);

            // When
            engine.LoadAll();
            var upgrade = CreatePackage("a", "1.1");
            engine.InstallOrUpgrade(upgrade);

            // Then
            Assert.AreEqual(PluginState.Loaded, engine.Get("a").State);
            Assert.AreEqual(upgrade.Manifest.Version, engine.Get("a").Package.Manifest.Version);
        }

        [Test]
        public void InstallOrUpgrade_WithHigherVersionOfExistingPluginHavingDependency_LoadsCorrectly()
        {
            // Given
            var pkgA = CreatePackage("a", "1.0");
            var pkgB = CreatePackage("b", "1.0", new Dependency {Name = "a"});
            var pkgC = CreatePackage("c", "1.0", new Dependency {Name = "b"});
            var factory = CreatePackageFactory(pkgA, pkgB, pkgC);
            var engine = CreatePluginEngine(packageFactory: factory);

            // When
            engine.LoadAll();
            var upgrade = CreatePackage("a", "1.1");
            engine.InstallOrUpgrade(upgrade);

            // Then
            Assert.AreEqual(PluginState.Loaded, engine.Get("a").State);
            Assert.AreEqual(PluginState.Loaded, engine.Get("b").State);
            Assert.AreEqual(PluginState.Loaded, engine.Get("c").State);
            Assert.AreEqual(upgrade.Manifest.Version, engine.Get("a").Package.Manifest.Version);
        }

        private IPackage CreatePackage(string name, string version, params Dependency[] dependencies)
        {
            var manifest = new Mock<IManifest>();
            manifest.SetupGet(m => m.Dependencies).Returns(dependencies);
            manifest.SetupGet(m => m.Name).Returns(name);
            manifest.SetupGet(m => m.Version).Returns(version);

            var package = new Mock<IPackage>();
            package.SetupGet(p => p.Manifest).Returns(manifest.Object);

            return package.Object;
        }

        private PluginEngine CreatePluginEngine(IConfiguration configuration = null,
            IFileSystem fileSystem = null,
            IJsonRpcClient jsonRpcClient = null,
            IPackageFactory packageFactory = null,
            IIsolatedEnvironmentFactory environmentFactory = null)
        {
            if (configuration == null)
            {
                configuration = CreateDefaultConfiguration();
            }

            if (fileSystem == null)
            {
                var fsMock = new Mock<IFileSystem>();
                fsMock.Setup(f => f.GetDirectory(It.IsAny<string>())).Returns(new Mock<IDirectory>().Object);

                var looseFile = new Mock<IFile>();
                looseFile.Setup(f => f.OpenWrite()).Returns(() => new MemoryStream());
                fsMock.Setup(f => f.GetFile(It.IsAny<string>())).Returns(looseFile.Object);

                fileSystem = fsMock.Object;
            }

            if (jsonRpcClient == null)
            {
                jsonRpcClient = new Mock<IJsonRpcClient>().Object;
            }

            if (packageFactory == null)
            {
                packageFactory = new Mock<IPackageFactory>().Object;
            }

            if (environmentFactory == null)
            {
                var envMock = new Mock<IIsolatedEnvironmentFactory>();
                envMock.Setup(e => e.CreateEnvironment(It.IsAny<IPackage>(), It.IsAny<IBootConfig>()))
                    .Returns(new Mock<IIsolatedEnvironment>().Object);

                environmentFactory = envMock.Object;
            }

            return new PluginEngine(configuration, fileSystem, jsonRpcClient, packageFactory, environmentFactory);
        }

        private IPackageFactory CreatePackageFactory(params IPackage[] packages)
        {
            var factory = new Mock<IPackageFactory>();
            factory.Setup(f => f.Scan()).Returns(packages);

            return factory.Object;
        }

        private IConfiguration CreateDefaultConfiguration()
        {
            var cfg = new Mock<IConfiguration>();
            cfg.SetupGet(c => c.ApplicationDataPath).Returns("/app-data");

            var httpCfg = new Mock<IHttpConfiguration>();
            httpCfg.SetupGet(ca => ca.Authentication).Returns(new Mock<IHttpAuthConfiguration>().Object);
            cfg.SetupGet(c => c.Http).Returns(httpCfg.Object);
            
            cfg.SetupGet(c => c.InstanceName).Returns("unit-test");

            var pluginCfg = new Mock<IPluginConfigurationCollection>();
            pluginCfg.SetupGet(p => p.BaseDirectory).Returns("/plugins/");
            cfg.SetupGet(c => c.Plugins).Returns(pluginCfg.Object);

            var rpcCfg = new Mock<IRpcConfiguration>();
            rpcCfg.SetupGet(c => c.PluginUriTemplate).Returns("{0}");
            cfg.SetupGet(c => c.Rpc).Returns(rpcCfg.Object);

            return cfg.Object;
        }
    }
}
