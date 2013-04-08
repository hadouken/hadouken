using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hadouken.Common.Messaging.Msmq
{
    [Component(ComponentType.Singleton)]
    public class MsmqMessageBusFactory : IMessageBusFactory
    {
        private IMessageBus _messageBus;

        public IMessageBus Create(string name)
        {
            if (_messageBus == null)
            {
                _messageBus = new MsmqMessageBus("msmq://localhost/hdkn");
                _messageBus.Load();
            }

            return _messageBus;
        }
    }
}
