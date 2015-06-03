using System;
using Hadouken.Common.Data;
using Hadouken.Common.IO;
using Hadouken.Common.Messaging;
using Hadouken.Extensions.AutoAdd.Data;
using Hadouken.Extensions.AutoAdd.Data.Models;
using Hadouken.Extensions.AutoAdd.Tests.Fixtures;
using NSubstitute;
using Xunit;

namespace Hadouken.Extensions.AutoAdd.Tests.Unit {
    public sealed class FolderScannerTests {
        public sealed class TheConstructor {
            [Fact]
            public void Should_Throw_If_File_System_Is_Null() {
                // Given, When
                var result = Record.Exception(() => new FolderScanner(null,
                    Substitute.For<IKeyValueStore>(),
                    Substitute.For<IAutoAddRepository>(),
                    Substitute.For<IMessageBus>()));

                // When
                Assert.IsType<ArgumentNullException>(result);
                Assert.Equal("fileSystem", ((ArgumentNullException) result).ParamName);
            }

            [Fact]
            public void Should_Throw_If_Key_Value_Store_Is_Null() {
                // Given, When
                var result = Record.Exception(() => new FolderScanner(Substitute.For<IFileSystem>(),
                    null,
                    Substitute.For<IAutoAddRepository>(),
                    Substitute.For<IMessageBus>()));

                // When
                Assert.IsType<ArgumentNullException>(result);
                Assert.Equal("keyValueStore", ((ArgumentNullException) result).ParamName);
            }

            [Fact]
            public void Should_Throw_If_Auto_Add_Repository_Is_Null() {
                // Given, When
                var result = Record.Exception(() => new FolderScanner(Substitute.For<IFileSystem>(),
                    Substitute.For<IKeyValueStore>(),
                    null,
                    Substitute.For<IMessageBus>()));

                // When
                Assert.IsType<ArgumentNullException>(result);
                Assert.Equal("autoAddRepository", ((ArgumentNullException) result).ParamName);
            }

            [Fact]
            public void Should_Throw_If_Message_Bus_Is_Null() {
                // Given, When
                var result = Record.Exception(() => new FolderScanner(Substitute.For<IFileSystem>(),
                    Substitute.For<IKeyValueStore>(),
                    Substitute.For<IAutoAddRepository>(),
                    null));

                // When
                Assert.IsType<ArgumentNullException>(result);
                Assert.Equal("messageBus", ((ArgumentNullException) result).ParamName);
            }
        }

        public sealed class TheScanMethod {
            [Fact]
            public void Should_Throw_If_Folder_Is_Null() {
                // Given
                var scanner = new FolderScannerFixture().CreateScanner();

                // When
                var exception = Record.Exception(() => scanner.Scan(null));

                // Then
                Assert.IsType<ArgumentNullException>(exception);
                Assert.Equal("folder", ((ArgumentNullException) exception).ParamName);
            }

            [Fact]
            public void Should_Not_Throw_If_Folder_Does_Not_Exist() {
                // Given
                var fixture = new FolderScannerFixture();
                var scanner = fixture.CreateScanner();

                // When
                var exception = Record.Exception(() => scanner.Scan(new Folder {Path = "C:\\Non-existing"}));

                // Then
                fixture.FileSystem.GetDirectory("C:\\Non-existing").Received(1);
                Assert.Null(exception);
            }
        }
    }
}