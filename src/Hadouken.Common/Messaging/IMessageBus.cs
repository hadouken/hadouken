using System;

namespace Hadouken.Common.Messaging
{
    public interface IMessageBus
    {
        void Publish<T>(T message) where T : class, IMessage;

        void Subscribe<T>(Action<T> callback) where T : IMessage;

        void Unsubscribe<T>(Action<T> callback) where T : IMessage;
    }
}