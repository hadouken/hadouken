using System;
using Hadouken.Common.Data;
using Hadouken.Extensions.HipChat.Config;
using Hadouken.Extensions.HipChat.Http;
using Hadouken.Extensions.HipChat.Tests.Fixtures;
using NSubstitute;
using Xunit;

namespace Hadouken.Extensions.HipChat.Tests.Unit
{
    public sealed class HipChatNotifierTests
    {
        public sealed class TheConstructor
        {
            [Fact]
            public void Should_Throw_Exception_If_Key_Value_Store_Is_Null()
            {
                // Given, When
                var exception = Record.Exception(() => new HipChatNotifier(null, Substitute.For<IHipChatClient>()));

                // Then
                Assert.IsType<ArgumentNullException>(exception);
                Assert.Equal("keyValueStore", ((ArgumentNullException) exception).ParamName);
            }

            [Fact]
            public void Should_Throw_Exception_If_HipChat_Client_Is_Null()
            {
                // Given, When
                var exception = Record.Exception(() => new HipChatNotifier(Substitute.For<IKeyValueStore>(), null));

                // Then
                Assert.IsType<ArgumentNullException>(exception);
                Assert.Equal("hipChatClient", ((ArgumentNullException) exception).ParamName);
            }
        }

        public sealed class TheCanNotifyMethod
        {
            [Fact]
            public void Should_Return_False_If_Config_Is_Null()
            {
                // Given
                var fixture = new HipChatNotifierFixture();
                fixture.KeyValueStore.Get<HipChatConfig>("hipchat.config").Returns(info => null);
                var notifier = fixture.CreateNotifier();

                // When
                var result = notifier.CanNotify();

                // Then
                Assert.False(result);
            }

            [Fact]
            public void Should_Return_True_If_Config_Is_Valid()
            {
                // Given
                var fixture = new HipChatNotifierFixture();
                var config = new HipChatConfig {AuthenticationToken = "auth-token", From = "Test", RoomId = "Room"};
                fixture.KeyValueStore.Get<HipChatConfig>("hipchat.config").Returns(config);
                var notifier = fixture.CreateNotifier();

                // When
                var result = notifier.CanNotify();

                // Then
                Assert.True(result);
            }
        }
    }
}
