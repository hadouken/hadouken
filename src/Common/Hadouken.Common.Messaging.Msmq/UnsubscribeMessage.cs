namespace Hadouken.Common.Messaging.Msmq
{
    public class UnsubscribeMessage : Message
    {
        public string Path { get; set; }
    }
}
