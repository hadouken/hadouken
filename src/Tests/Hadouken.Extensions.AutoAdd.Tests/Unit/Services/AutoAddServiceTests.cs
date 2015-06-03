using System;
using System.Linq;
using Hadouken.Extensions.AutoAdd.Data.Models;
using Hadouken.Extensions.AutoAdd.Services;
using Hadouken.Extensions.AutoAdd.Tests.Fixtures;
using NSubstitute;
using Xunit;

namespace Hadouken.Extensions.AutoAdd.Tests.Unit.Services {
    public sealed class AutoAddServiceTests {
        public sealed class TheConstructor {
            [Fact]
            public void Should_Throw_Exception_If_Auto_Add_Repository_Is_Null() {
                // Given, When
                var exception = Record.Exception(() => new AutoAddService(null));

                // Then
                Assert.IsType<ArgumentNullException>(exception);
                Assert.Equal("autoAddRepository", ((ArgumentNullException) exception).ParamName);
            }
        }

        public sealed class TheCreateFolderMethod {
            [Fact]
            public void Should_Throw_Exception_If_Folder_Is_Null() {
                // Given
                var service = new AutoAddServiceFixture().CreateAutoAddService();

                // When
                var exception = Record.Exception(() => service.CreateFolder(null));

                // Then
                Assert.IsType<ArgumentNullException>(exception);
                Assert.Equal("folder", ((ArgumentNullException) exception).ParamName);
            }

            [Fact]
            public void Should_Return_The_Folder_Passed_As_Argument() {
                // Given
                var service = new AutoAddServiceFixture().CreateAutoAddService();

                // When
                var folder = service.CreateFolder(new Folder {Path = "<some path>"});

                // Then
                Assert.Equal("<some path>", folder.Path);
            }
        }

        public sealed class TheUpdateFolderMethod {
            [Fact]
            public void Should_Throw_Exception_If_Folder_Is_Null() {
                // Given
                var service = new AutoAddServiceFixture().CreateAutoAddService();

                // When
                var exception = Record.Exception(() => service.UpdateFolder(null));

                // Then
                Assert.IsType<ArgumentNullException>(exception);
                Assert.Equal("folder", ((ArgumentNullException) exception).ParamName);
            }

            [Fact]
            public void Should_Call_Update_Folder_On_Repository() {
                // Given
                var fixture = new AutoAddServiceFixture();
                var service = fixture.CreateAutoAddService();

                // When
                service.UpdateFolder(new Folder());

                // Then
                fixture.AutoAddRepository.Received(1).UpdateFolder(Arg.Any<Folder>());
            }
        }

        public sealed class TheGetAllMethod {
            [Fact]
            public void Should_Return_An_Empty_List_If_Repository_Is_Null() {
                // Given
                var fixture = new AutoAddServiceFixture();
                fixture.AutoAddRepository.GetFolders().Returns(info => null);
                var service = fixture.CreateAutoAddService();

                // When
                var result = service.GetFolders();

                // Then
                Assert.Equal(Enumerable.Empty<Folder>(), result);
            }

            [Fact]
            public void Should_Return_An_List_Of_Folders() {
                // Given
                var fixture = new AutoAddServiceFixture();
                fixture.AutoAddRepository.GetFolders().Returns(info => new[] {new Folder()});
                var service = fixture.CreateAutoAddService();

                // When
                var result = service.GetFolders();

                // Then
                Assert.Equal(1, result.Count());
            }
        }

        public sealed class TheDeleteFolderMethod {
            [Fact]
            public void Should_Call_Delete_Folder_On_Repository() {
                // Given
                var fixture = new AutoAddServiceFixture();
                var service = fixture.CreateAutoAddService();

                // When
                service.DeleteFolder(10);

                // Then
                fixture.AutoAddRepository.Received(1).DeleteFolder(10);
            }
        }
    }
}