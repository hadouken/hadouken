namespace Hadouken.Common.Messaging
{
    /// <summary>
    /// This is a marker interface for a message handler. Use the generic
    /// <see cref="IMessageHandler{TMessage}"/> instead.
    /// </summary>
    public interface IMessageHandler
    {
    }

    public interface IMessageHandler<in TMessage> : IMessageHandler
        where TMessage : IMessage
    {
        void Handle(TMessage message);
    }
}
