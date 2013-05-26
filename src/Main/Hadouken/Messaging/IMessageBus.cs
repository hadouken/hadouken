using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hadouken.Messaging
{
    public interface IMessageBus
    {
        Task Send<TMessage>(Action<TMessage> builder) where TMessage : class, IMessage;

        void Subscribe<TMessage>(Action<TMessage> callback) where TMessage : IMessage;
    }
}
