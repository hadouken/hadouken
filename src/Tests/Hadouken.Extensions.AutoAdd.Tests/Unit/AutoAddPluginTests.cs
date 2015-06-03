using System;
using Hadouken.Extensions.AutoAdd.Data.Models;
using Hadouken.Extensions.AutoAdd.Tests.Fixtures;
using NSubstitute;
using Xunit;

namespace Hadouken.Extensions.AutoAdd.Tests.Unit {
    public sealed class AutoAddPluginTests {
        public sealed class TheConstructor {
            [Fact]
            public void Should_Throw_Exception_If_Logger_Is_Null() {
                // Given
                var fixture = new AutoAddPluginFixture {Logger = null};

                // When
                var exception = Record.Exception(() => fixture.CreateAutoAddPlugin());

                // Then
                Assert.IsType<ArgumentNullException>(exception);
                Assert.Equal("logger", ((ArgumentNullException) exception).ParamName);
            }

            [Fact]
            public void Should_Throw_Exception_If_Timer_Factory_Is_Null() {
                // Given
                var fixture = new AutoAddPluginFixture {TimerFactory = null};

                // When
                var exception = Record.Exception(() => fixture.CreateAutoAddPlugin());

                // Then
                Assert.IsType<ArgumentNullException>(exception);
                Assert.Equal("timerFactory", ((ArgumentNullException) exception).ParamName);
            }

            [Fact]
            public void Should_Throw_Exception_If_Repository_Is_Null() {
                // Given
                var fixture = new AutoAddPluginFixture {AutoAddRepository = null};

                // When
                var exception = Record.Exception(() => fixture.CreateAutoAddPlugin());

                // Then
                Assert.IsType<ArgumentNullException>(exception);
                Assert.Equal("repository", ((ArgumentNullException) exception).ParamName);
            }

            [Fact]
            public void Should_Throw_Exception_If_Folder_Scanner_Is_Null() {
                // Given
                var fixture = new AutoAddPluginFixture {FolderScanner = null};

                // When
                var exception = Record.Exception(() => fixture.CreateAutoAddPlugin());

                // Then
                Assert.IsType<ArgumentNullException>(exception);
                Assert.Equal("folderScanner", ((ArgumentNullException) exception).ParamName);
            }
        }

        public sealed class TheLoadMethod {
            [Fact]
            public void Should_Start_Timer() {
                // Given
                var fixture = new AutoAddPluginFixture();
                var plugin = fixture.CreateAutoAddPlugin();

                // When
                plugin.Load();

                // Then
                fixture.Timer.Received(1).Start();
            }
        }

        public sealed class TheUnloadMethod {
            [Fact]
            public void Should_Stop_Timer() {
                // Given
                var fixture = new AutoAddPluginFixture();
                var plugin = fixture.CreateAutoAddPlugin();

                // When
                plugin.Unload();

                // Then
                fixture.Timer.Received(1).Stop();
            }
        }

        public sealed class TheCheckFoldersMethod {
            [Fact]
            public void Should_Scan_Each_Folder_From_Repository() {
                // Given
                var fixture = new AutoAddPluginFixture();
                var plugin = fixture.CreateAutoAddPlugin();

                fixture.AutoAddRepository.GetFolders().Returns(new[] {new Folder(), new Folder()});

                // When
                plugin.CheckFolders();

                // Then
                fixture.FolderScanner.Received(2).Scan(Arg.Any<Folder>());
            }
        }
    }
}