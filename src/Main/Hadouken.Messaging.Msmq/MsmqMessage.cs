using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hadouken.Common.Messaging;

namespace Hadouken.Messaging.Msmq
{
    public class MsmqMessage<TMessage> where TMessage : Message
    {
        public string TypeName { get; set; }

        public TMessage Message { get; set; }
    }
}
