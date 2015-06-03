using System;
using Hadouken.Common.Data;
using Hadouken.Common.Extensibility.Notifications;
using Hadouken.Extensions.Kodi.Config;
using Hadouken.Extensions.Kodi.Http;
using Hadouken.Extensions.Kodi.Tests.Fixtures;
using NSubstitute;
using Xunit;

namespace Hadouken.Extensions.Kodi.Tests.Unit {
    public sealed class KodiNotifierTests {
        public sealed class TheConstructor {
            [Fact]
            public void Should_Throw_Exception_If_Kodi_Client_Is_Null() {
                // Given, When
                var exception = Record.Exception(() => new KodiNotifier(null, Substitute.For<IKeyValueStore>()));

                // Then
                Assert.IsType<ArgumentNullException>(exception);
                Assert.Equal("kodiClient", ((ArgumentNullException) exception).ParamName);
            }

            [Fact]
            public void Should_Throw_Exception_If_Key_Value_Store_Is_Null() {
                // Given, When
                var exception = Record.Exception(() => new KodiNotifier(Substitute.For<IKodiClient>(), null));

                // Then
                Assert.IsType<ArgumentNullException>(exception);
                Assert.Equal("keyValueStore", ((ArgumentNullException) exception).ParamName);
            }
        }

        public sealed class TheCanNotifyMethod {
            [Fact]
            public void Should_Return_False_If_Config_Is_Null() {
                // Given
                var notifier = new KodiNotifierFixture().CreateNotifier();

                // When
                var result = notifier.CanNotify();

                // Then
                Assert.False(result);
            }

            [Fact]
            public void Should_Return_False_If_Url_Is_Null() {
                // Given
                var fixture = new KodiNotifierFixture();
                fixture.KeyValueStore.Get<KodiConfig>("kodi.config").Returns(new KodiConfig {Url = null});
                var notifier = fixture.CreateNotifier();

                // When
                var result = notifier.CanNotify();

                // Then
                Assert.False(result);
            }

            [Fact]
            public void Should_Return_True_If_Url_Is_Not_Null_And_Enable_Authentication_Is_False() {
                // Given
                var fixture = new KodiNotifierFixture();
                fixture.KeyValueStore.Get<KodiConfig>("kodi.config")
                    .Returns(new KodiConfig {Url = new Uri("http://test.uri")});
                var notifier = fixture.CreateNotifier();

                // When
                var result = notifier.CanNotify();

                // Then
                Assert.True(result);
            }

            [Fact]
            public void Should_Return_False_If_Url_Is_Not_Null_And_Enable_Authentication_Is_True_And_UserName_Is_Null() {
                // Given
                var fixture = new KodiNotifierFixture();
                var config = new KodiConfig {
                    Url = new Uri("http://test.uri"),
                    EnableAuthentication = true,
                    Password = "pwd"
                };
                fixture.KeyValueStore.Get<KodiConfig>("kodi.config").Returns(config);
                var notifier = fixture.CreateNotifier();

                // When
                var result = notifier.CanNotify();

                // Then
                Assert.False(result);
            }

            [Fact]
            public void Should_Return_False_If_Url_Is_Not_Null_And_Enable_Authentication_Is_True_And_Password_Is_Null() {
                // Given
                var fixture = new KodiNotifierFixture();
                var config = new KodiConfig {
                    Url = new Uri("http://test.uri"),
                    EnableAuthentication = true,
                    UserName = "user"
                };
                fixture.KeyValueStore.Get<KodiConfig>("kodi.config").Returns(config);
                var notifier = fixture.CreateNotifier();

                // When
                var result = notifier.CanNotify();

                // Then
                Assert.False(result);
            }

            [Fact]
            public void
                Should_Return_True_If_Url_Is_Not_Null_And_Enable_Authentication_Is_True_And_UserName_And_Password_Is_Not_Null
                () {
                // Given
                var fixture = new KodiNotifierFixture();
                var config = new KodiConfig {
                    Url = new Uri("http://test.uri"),
                    EnableAuthentication = true,
                    UserName = "user",
                    Password = "pwd"
                };
                fixture.KeyValueStore.Get<KodiConfig>("kodi.config").Returns(config);
                var notifier = fixture.CreateNotifier();

                // When
                var result = notifier.CanNotify();

                // Then
                Assert.True(result);
            }
        }

        public sealed class TheNotifyMethod {
            [Fact]
            public void Should_Throw_Exception_If_Notification_Is_Null() {
                // Given
                var notifier = new KodiNotifierFixture().CreateNotifier();

                // When
                var exception = Record.Exception(() => notifier.Notify(null));

                // Then
                Assert.IsType<ArgumentNullException>(exception);
                Assert.Equal("notification", ((ArgumentNullException) exception).ParamName);
            }

            [Fact]
            public void Should_Call_Kodi_Client_With_Title_And_Message() {
                // Given
                var fixture = new KodiNotifierFixture();
                var notifier = fixture.CreateNotifier();

                // When
                notifier.Notify(new Notification(NotificationType.Test, "Test title", "Test message"));

                // Then
                fixture.KodiClient.Received(1).ShowNotification("Test title", "Test message");
            }
        }
    }
}