using System;
using Hadouken.Fx;
using Hadouken.JsonRpc;
using Hadouken.Plugins;
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
            public void Should_Throw_ArgumentNullException_If_PluginEngine_Is_Null()
            {
                Assert.Throws<ArgumentNullException>(() => new PluginsService(null));
            }
        }

        public class TheListPluginsMethod
        {
            [Fact]
            public void Should_Return_An_Array_With_Plugin_Names()
            {
                // Given
                var engine = Substitute.For<IPluginEngine>();
                var manager = Substitute.For<IPluginManager>();
                manager.Manifest.Name.Returns("TestPlugin");
                engine.GetAll().Returns(new[] {manager});
                var service = new PluginsService(engine);

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
                var engine = Substitute.For<IPluginEngine>();
                engine.Get("non-existent").Returns(info => null);
                var service = new PluginsService(engine);

                // When
                var result = service.GetVersion("non-existent");

                // Then
                Assert.Null(result);
            }

            [Fact]
            public void Should_Return_Correct_Version_If_Plugin_Exists()
            {
                // Given
                var engine = Substitute.For<IPluginEngine>();
                var manager = Substitute.For<IPluginManager>();
                manager.Manifest.Version.Returns(new SemanticVersion("1.0.0"));
                engine.Get("Plugin").Returns(manager);
                var service = new PluginsService(engine);

                // When
                var result = service.GetVersion("Plugin");

                // Then
                Assert.Equal("1.0.0", result);
            }
        }
    }
}
