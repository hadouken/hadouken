using Hadouken.Common.Data;
using Hadouken.Extensions.HipChat.Http;
using NSubstitute;

namespace Hadouken.Extensions.HipChat.Tests.Fixtures {
    internal sealed class HipChatNotifierFixture {
        public HipChatNotifierFixture() {
            this.KeyValueStore = Substitute.For<IKeyValueStore>();
            this.HipChatClient = Substitute.For<IHipChatClient>();
        }

        public IKeyValueStore KeyValueStore { get; set; }
        public IHipChatClient HipChatClient { get; set; }

        public HipChatNotifier CreateNotifier() {
            return new HipChatNotifier(this.KeyValueStore, this.HipChatClient);
        }
    }
}