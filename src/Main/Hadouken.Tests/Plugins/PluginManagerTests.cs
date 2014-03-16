using System;
using Hadouken.Configuration;
using Hadouken.Fx;
using Hadouken.Fx.IO;
using Hadouken.Plugins;
using Hadouken.Plugins.Isolation;
using Hadouken.Plugins.Metadata;
using NSubstitute;
using Xunit;

namespace Hadouken.Tests.Plugins
{
    public class PluginManagerTests
    {
        public class TheConstructor
        {
            [Fact]
            public void Should_Throw_ArgumentNullException_If_Configuration_Is_Null()
            {
                // Given, When, Then
                Assert.Throws<ArgumentNullException>(
                    () =>
                        new PluginManager(null,
                            Substitute.For<IDirectory>(),
                            Substitute.For<IIsolatedEnvironment>(),
                            Substitute.For<IManifest>()));
            }

            [Fact]
            public void Should_Throw_ArgumentNullException_If_BaseDirectory_Is_Null()
            {
                // Given, When, Then
                Assert.Throws<ArgumentNullException>(
                    () =>
                        new PluginManager(Substitute.For<IConfiguration>(),
                            null,
                            Substitute.For<IIsolatedEnvironment>(),
                            Substitute.For<IManifest>()));
            }

            [Fact]
            public void Should_Throw_ArgumentNullException_If_Environment_Is_Null()
            {
                // Given, When, Then
                Assert.Throws<ArgumentNullException>(
                    () =>
                        new PluginManager(Substitute.For<IConfiguration>(),
                            Substitute.For<IDirectory>(),
                            null,
                            Substitute.For<IManifest>()));
            }

            [Fact]
            public void Should_Throw_ArgumentNullException_If_Manifest_Is_Null()
            {
                // Given, When, Then
                Assert.Throws<ArgumentNullException>(
                    () =>
                        new PluginManager(Substitute.For<IConfiguration>(),
                            Substitute.For<IDirectory>(),
                            Substitute.For<IIsolatedEnvironment>(),
                            null));
            }

            [Fact]
            public void Should_Set_State_To_Unloaded()
            {
                // Given, When
                var manager = new PluginManager(Substitute.For<IConfiguration>(),
                    Substitute.For<IDirectory>(),
                    Substitute.For<IIsolatedEnvironment>(),
                    Substitute.For<IManifest>());

                // Then
                Assert.Equal(PluginState.Unloaded, manager.State);
            }
        }

        public class TheGetMemoryUsageMethod
        {
            [Fact]
            public void Should_Not_Call_GetMemoryUsage_On_IsolatedEnvironment_When_State_Is_Not_Loaded()
            {
                // Given
                var environment = Substitute.For<IIsolatedEnvironment>();
                var manager = new PluginManager(Substitute.For<IConfiguration>(),
                    Substitute.For<IDirectory>(),
                    environment,
                    Substitute.For<IManifest>());

                // When
                var usage = manager.GetMemoryUsage();

                // Then
                environment.DidNotReceive().GetMemoryUsage();
                Assert.Equal(-1, usage);
            }

            [Fact]
            public void Should_Call_GetMemoryUsage_On_IsolatedEnvironment_When_State_Is_Loaded()
            {
                // Given
                var environment = Substitute.For<IIsolatedEnvironment>();
                var manager = new PluginManager(Substitute.For<IConfiguration>(),
                    Substitute.For<IDirectory>(),
                    environment,
                    Substitute.For<IManifest>());

                // When
                manager.Load();
                manager.GetMemoryUsage();

                // Then
                environment.Received().GetMemoryUsage();
            }
        }

        public class TheLoadMethod
        {
            [Fact]
            public void Should_Set_State_To_Loaded_If_IsolatedEnvironment_Load_Succeeds()
            {
                // Given
                var environment = Substitute.For<IIsolatedEnvironment>();
                var manager = new PluginManager(Substitute.For<IConfiguration>(),
                    Substitute.For<IDirectory>(),
                    environment,
                    Substitute.For<IManifest>());

                // When
                manager.Load();

                // Then
                environment.Received().Load(Arg.Any<PluginConfiguration>());
                Assert.Equal(PluginState.Loaded, manager.State);
            }

            [Fact]
            public void Should_Set_State_To_Error_If_IsolatedEnvironment_Load_Throws()
            {
                // Given
                var environment = Substitute.For<IIsolatedEnvironment>();
                environment.When(e => e.Load(Arg.Any<PluginConfiguration>())).Do(c => { throw new Exception(); });
                var manager = new PluginManager(Substitute.For<IConfiguration>(),
                    Substitute.For<IDirectory>(),
                    environment,
                    Substitute.For<IManifest>());

                // When
                manager.Load();

                // Then
                Assert.Equal(PluginState.Error, manager.State);
            }
        }

        public class TheUnloadMethod
        {
            [Fact]
            public void Should_Set_State_To_Unloaded_If_IsolatedEnvironment_Unload_Succeeds()
            {
                // Given
                var environment = Substitute.For<IIsolatedEnvironment>();
                var manager = new PluginManager(Substitute.For<IConfiguration>(),
                    Substitute.For<IDirectory>(),
                    environment,
                    Substitute.For<IManifest>());

                // When
                manager.Unload();

                // Then
                environment.Received().Unload();
                Assert.Equal(PluginState.Unloaded, manager.State);
            }

            [Fact]
            public void Should_Set_State_To_Error_If_IsolatedEnvironment_Unload_Throws()
            {
                // Given
                var environment = Substitute.For<IIsolatedEnvironment>();
                environment.When(e => e.Unload()).Do(c => { throw new Exception(); });
                var manager = new PluginManager(Substitute.For<IConfiguration>(),
                    Substitute.For<IDirectory>(),
                    environment,
                    Substitute.For<IManifest>());

                // When
                manager.Unload();

                // Then
                Assert.Equal(PluginState.Error, manager.State);
            }
        }
    }
}
