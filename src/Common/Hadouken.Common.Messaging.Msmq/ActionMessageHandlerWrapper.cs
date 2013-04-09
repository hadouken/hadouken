using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hadouken.Common.Messaging.Msmq
{
    public class ActionMessageHandlerWrapper<TMessage> : IMessageHandler<TMessage> where TMessage : IMessage
    {
        private readonly Action<TMessage> _callback;

        public ActionMessageHandlerWrapper(Action<TMessage> callback)
        {
            _callback = callback;
        }

        public void Handle(TMessage message)
        {
            _callback(message);
        }
    }
}
