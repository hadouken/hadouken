using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hadouken.Common.Messaging;
using System.Messaging;
using Hadouken.Messaging.Msmq.Formatters;

namespace Hadouken.Messaging.Msmq
{
    public interface IMsmqMessageBus : IMessageBus
    {
        IDictionary<string, MessageQueue> ExternalQueues { get; } 
    }
}
