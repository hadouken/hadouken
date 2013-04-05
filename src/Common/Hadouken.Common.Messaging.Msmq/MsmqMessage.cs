namespace Hadouken.Common.Messaging.Msmq
{
    public class MsmqMessage<TMessage> where TMessage : Message
    {
        public string TypeName { get; set; }

        public TMessage Message { get; set; }
    }
}
