using Hadouken.Extensions.HipChat.Config;

namespace Hadouken.Extensions.HipChat.Http {
    public interface IHipChatClient {
        void SendMessage(HipChatConfig config, string message);
    }
}