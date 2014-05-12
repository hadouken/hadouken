namespace Hadouken.Messaging
{
    public interface IMessageHandler<in T> where T : Message
    {
        void Handle(T message);
    }
}