using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hadouken.Common.Messaging.Msmq
{
    internal class MessageHandlerWrapper<TMessage> : IMessageHandlerWrapper where TMessage : Message
    {
        private readonly IMessageHandler<TMessage> _handler; 

        public MessageHandlerWrapper(IMessageHandler<TMessage> handler)
        {
            _handler = handler;
        } 

        public void Execute(Message message)
        {
            _handler.Handle((TMessage)message);
        }
    }
}
