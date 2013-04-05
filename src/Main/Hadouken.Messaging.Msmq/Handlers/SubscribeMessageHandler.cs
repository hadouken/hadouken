using System;
using System.Collections.Generic;
using System.Linq;
using System.Messaging;
using System.Text;
using Hadouken.Common.Messaging;

namespace Hadouken.Messaging.Msmq.Handlers
{
    public class SubscribeMessageHandler : IMessageHandler<SubscribeMessage>
    {
        private readonly IMsmqMessageBus _messageBus;

        public SubscribeMessageHandler(IMsmqMessageBus messageBus)
        {
            _messageBus = messageBus;
        }

        public void Handle(SubscribeMessage message)
        {
            if(message == null)
                throw new ArgumentNullException("message");

            if (_messageBus.ExternalQueues.ContainsKey(message.Path))
                throw new ArgumentException("Queue is already registered with this message bus.");

            _messageBus.ExternalQueues.Add(message.Path, new MessageQueue(message.Path));
        }
    }
}
