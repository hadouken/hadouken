using System;
using System.Net;
using System.Net.Http;
using Hadouken.Common.Logging;
using Hadouken.Common.Net;
using Hadouken.Extensions.HipChat.Config;
using Hadouken.Extensions.HipChat.Http;
using Hadouken.Extensions.HipChat.Tests.Fixtures;
using NSubstitute;
using Xunit;

namespace Hadouken.Extensions.HipChat.Tests.Unit.Http
{
    public sealed class HipChatClientTests
    {
        public sealed class TheConstructor
        {
            [Fact]
            public void Should_Throw_Exception_If_Logger_Is_Null()
            {
                // Given, When
                var exception = Record.Exception(() => new HipChatClient(null, Substitute.For<IHttpClient>()));

                // Then
                Assert.IsType<ArgumentNullException>(exception);
                Assert.Equal("logger", ((ArgumentNullException) exception).ParamName);
            }

            [Fact]
            public void Should_Throw_Exception_If_Http_Client_Is_Null()
            {
                // Given, When
                var exception = Record.Exception(() => new HipChatClient(Substitute.For<ILogger<HipChatClient>>(), null));

                // Then
                Assert.IsType<ArgumentNullException>(exception);
                Assert.Equal("httpClient", ((ArgumentNullException) exception).ParamName);
            }
        }

        public sealed class TheSendMessageMethod
        {
            [Fact]
            public void Should_Throw_Exception_If_Config_Is_Null()
            {
                // Given
                var client = new HipChatClientFixture().CreateClient();

                // When
                var exception = Record.Exception(() => client.SendMessage(null, "Message"));

                // Then
                Assert.IsType<ArgumentNullException>(exception);
                Assert.Equal("config", ((ArgumentNullException) exception).ParamName);
            }

            [Fact]
            public void Should_Throw_Exception_If_Message_Is_Null()
            {
                // Given
                var client = new HipChatClientFixture().CreateClient();

                // When
                var exception = Record.Exception(() => client.SendMessage(new HipChatConfig(), null));

                // Then
                Assert.IsType<ArgumentNullException>(exception);
                Assert.Equal("message", ((ArgumentNullException) exception).ParamName);
            }

            [Fact]
            public void Should_Append_Auth_Token_To_Uri()
            {
                // Given
                var expectedUri = new Uri("https://api.hipchat.com/v1/rooms/message?auth_token=token");
                var fixture = new HipChatClientFixture();
                var client = fixture.CreateClient();

                // When
                client.SendMessage(new HipChatConfig {AuthenticationToken = "token"}, "Message");

                // Then
                fixture.HttpClient.Received(1).PostAsync(expectedUri, Arg.Any<HttpContent>());
            }

            [Fact]
            public void Should_Log_If_Post_Did_Not_Return_Success_Status()
            {
                var fixture = new HipChatClientFixture();
                fixture.HttpResponseMessage.StatusCode = HttpStatusCode.InternalServerError;
                var client = fixture.CreateClient();
                var config = new HipChatConfig {AuthenticationToken = "token", From = "test", RoomId = "rid"};
                
                // When
                client.SendMessage(config, "Message");

                // Then
                fixture.Logger.Received(1).Error(Arg.Any<string>(), HttpStatusCode.InternalServerError);
            }
        }
    }
}
