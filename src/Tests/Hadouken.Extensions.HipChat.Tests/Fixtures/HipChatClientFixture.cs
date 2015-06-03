using System;
using System.Net.Http;
using System.Threading.Tasks;
using Hadouken.Common.Logging;
using Hadouken.Common.Net;
using Hadouken.Extensions.HipChat.Http;
using NSubstitute;

namespace Hadouken.Extensions.HipChat.Tests.Fixtures {
    internal sealed class HipChatClientFixture {
        public HipChatClientFixture() {
            this.Logger = Substitute.For<ILogger<HipChatClient>>();
            this.HttpResponseMessage = new HttpResponseMessage();
            this.HttpClient = Substitute.For<IHttpClient>();
            this.HttpClient.PostAsync(Arg.Any<Uri>(), Arg.Any<HttpContent>())
                .Returns(Task.FromResult(this.HttpResponseMessage));
        }

        public HttpResponseMessage HttpResponseMessage { get; set; }
        public ILogger<HipChatClient> Logger { get; set; }
        public IHttpClient HttpClient { get; set; }

        public HipChatClient CreateClient() {
            return new HipChatClient(this.Logger, this.HttpClient);
        }
    }
}