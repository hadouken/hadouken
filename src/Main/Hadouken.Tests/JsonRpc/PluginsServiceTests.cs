using System;
using Hadouken.Configuration;
using Hadouken.Fx;
using Hadouken.JsonRpc;
using Hadouken.Plugins;
using Hadouken.Security;
using Hadouken.SemVer;
using NSubstitute;
using Xunit;

namespace Hadouken.Tests.JsonRpc
{
    public class PluginsServiceTests
    {
        public class TheConstructor
        {
            [Fact]
            public void Should_Throw_ArgumentNullException_If_Configuration_Is_Null()
            {
                // Given, When
                var exception = Assert.Throws<ArgumentNullException>(() => new PluginsService(null,
                    Substitute.For<IAuthenticationManager>(),
                    Substitute.For<IPluginEngine>(),
                    Substitute.For<IPackageReader>()));

                // Then
                Assert.Equal("configuration", exception.ParamName);
            }

            [Fact]
            public void Should_Throw_ArgumentNullException_If_AuthenticationManager_Is_Null()
            {
                // Given, When
                var exception = Assert.Throws<ArgumentNullException>(() => new PluginsService(Substitute.For<IConfiguration>(),
                    null,
                    Substitute.For<IPluginEngine>(),
                    Substitute.For<IPackageReader>()));

                // Then
                Assert.Equal("authManager", exception.ParamName);
            }

            [Fact]
            public void Should_Throw_ArgumentNullException_If_PluginEngine_Is_Null()
            {
                // Given, When
                var exception = Assert.Throws<ArgumentNullException>(() => new PluginsService(Substitute.For<IConfiguration>(),
                    Substitute.For<IAuthenticationManager>(),
                    null,
                    Substitute.For<IPackageReader>()));

                // Then
                Assert.Equal("pluginEngine", exception.ParamName);
            }

            [Fact]
            public void Should_Throw_ArgumentNullException_If_PackageReader_Is_Null()
            {
                // Given, When
                var exception = Assert.Throws<ArgumentNullException>(() => new PluginsService(Substitute.For<IConfiguration>(),
                    Substitute.For<IAuthenticationManager>(),
                    Substitute.For<IPluginEngine>(),
                    null));

                // Then
                Assert.Equal("packageReader", exception.ParamName);
            }
        }

        public class TheListPluginsMethod
        {
            [Fact]
            public void Should_Return_An_Array_With_Plugin_Names()
            {
                // Given
                var conf = Substitute.For<IConfiguration>();
                var authMan = Substitute.For<IAuthenticationManager>();
                var engine = Substitute.For<IPluginEngine>();
                var reader = Substitute.For<IPackageReader>();
                var manager = Substitute.For<IPluginManager>();
                manager.Manifest.Name.Returns("TestPlugin");
                engine.GetAll().Returns(new[] {manager});
                var service = new PluginsService(conf, authMan, engine, reader);

                // When
                var result = service.ListPlugins();

                // Then
                Assert.Equal(result, new[] {"TestPlugin"});
            }
        }

        public class TheGetVersionMethod
        {
            [Fact]
            public void Should_Return_Null_If_Plugin_Is_Null()
            {
                // Given
                var conf = Substitute.For<IConfiguration>();
                var authMan = Substitute.For<IAuthenticationManager>();
                var engine = Substitute.For<IPluginEngine>();
                var reader = Substitute.For<IPackageReader>();
                engine.Get("non-existent").Returns(info => null);
                var service = new PluginsService(conf, authMan, engine, reader);

                // When
                var result = service.GetVersion("non-existent");

                // Then
                Assert.Null(result);
            }

            [Fact]
            public void Should_Return_Correct_Version_If_Plugin_Exists()
            {
                // Given
                var conf = Substitute.For<IConfiguration>();
                var authMan = Substitute.For<IAuthenticationManager>();
                var engine = Substitute.For<IPluginEngine>();
                var reader = Substitute.For<IPackageReader>();
                var manager = Substitute.For<IPluginManager>();
                manager.Manifest.Version.Returns(new SemanticVersion("1.0.0"));
                engine.Get("Plugin").Returns(manager);
                var service = new PluginsService(conf, authMan, engine, reader);

                // When
                var result = service.GetVersion("Plugin");

                // Then
                Assert.Equal("1.0.0", result);
            }
        }

        public class TheInstallMethod
        {
            [Fact]
            public void Should_Throw_ArgumentNullException_If_Package_Data_Is_Null()
            {
                // Given
                var conf = Substitute.For<IConfiguration>();
                var authMan = Substitute.For<IAuthenticationManager>();
                var engine = Substitute.For<IPluginEngine>();
                var reader = Substitute.For<IPackageReader>();
                var service = new PluginsService(conf, authMan, engine, reader);

                // When
                var exception = Assert.Throws<ArgumentNullException>(() => service.Install("test", null));

                // Then
                Assert.Equal("base64EncodedPackage", exception.ParamName);
            }
        }
    }
}
