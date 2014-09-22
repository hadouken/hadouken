namespace Hadouken.Extensions.HipChat.Config
{
    public sealed class HipChatConfig
    {
        public string AuthenticationToken { get; set; }

        public string RoomId { get; set; }

        public string From { get; set; }

        public bool Notify { get; set; }
    }
}
