using System.Collections.Generic;
using System.Messaging;

namespace Hadouken.Common.Messaging.Msmq
{
    public interface IMsmqMessageBus : IMessageBus
    {
        IDictionary<string, MessageQueue> ExternalQueues { get; } 
    }
}
