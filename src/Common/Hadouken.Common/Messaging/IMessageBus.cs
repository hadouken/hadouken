using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hadouken.Common.Messaging
{
    public interface IMessageBus
    {
        void Load();
        void Unload();

        void Publish<TMessage>(TMessage message) where TMessage : Message;
        void Subscribe<TMessage>(Action<TMessage> callback) where TMessage : Message;
    }
}
