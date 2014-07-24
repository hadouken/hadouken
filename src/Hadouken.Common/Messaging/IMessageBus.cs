using System;

namespace Hadouken.Common.Messaging
{
    public interface IMessageBus
    {
        void Publish(IMessage message);

        void Subscribe<T>(Action<T> callback) where T : IMessage;

        void Unsubscribe<T>(Action<T> callback) where T : IMessage;
    }
}