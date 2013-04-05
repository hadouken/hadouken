using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hadouken.Common.Messaging.Msmq
{
    [Component]
    public class MsmqMessageBusFactory : IMessageBusFactory
    {
        private readonly IDictionary<string, IMessageBus> _messageBusses =
            new Dictionary<string, IMessageBus>(StringComparer.InvariantCultureIgnoreCase);

        public IMessageBus Create(string name)
        {
            if (!_messageBusses.ContainsKey(name))
                _messageBusses.Add(name, new MsmqMessageBus(".\\private$\\" + name));

            return _messageBusses[name];
        }
    }
}
