using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hadouken.Common.Messaging;

namespace Hadouken.Messaging.Msmq.Handlers
{
    public class UnsubscribeMessageHandler : IMessageHandler<UnsubscribeMessage>
    {
        private readonly IMsmqMessageBus _messageBus;

        public UnsubscribeMessageHandler(IMsmqMessageBus messageBus)
        {
            _messageBus = messageBus;
        }

        public void Handle(UnsubscribeMessage message)
        {
            if(message == null)
                throw new ArgumentNullException("message");
        }
    }
}
