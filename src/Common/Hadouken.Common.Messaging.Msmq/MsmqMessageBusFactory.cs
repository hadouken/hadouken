using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hadouken.Common.Messaging.Msmq
{
    [Component(ComponentType.Singleton)]
    public class MsmqMessageBusFactory : IMessageBusFactory
    {
        private readonly IDictionary<string, IMessageBus> _messageBusses = new Dictionary<string, IMessageBus>();
 
        public IMessageBus Create(string name)
        {
            if (!_messageBusses.ContainsKey(name))
            {
                var messageBus = new MsmqMessageBus("msmq://localhost/" + name);
                messageBus.Load();

                _messageBusses.Add(name, messageBus);
            }

            return _messageBusses[name];
        }
    }
}
