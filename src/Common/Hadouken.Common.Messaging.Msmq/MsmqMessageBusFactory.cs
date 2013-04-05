using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hadouken.Common.Messaging.Msmq
{
    [Component]
    public class MsmqMessageBusFactory : IMessageBusFactory
    {
        public IMessageBus Create(string name)
        {
            var bus = new MsmqMessageBus("msmq://localhost/" + name);
            bus.Load();

            return bus;
        }
    }
}
