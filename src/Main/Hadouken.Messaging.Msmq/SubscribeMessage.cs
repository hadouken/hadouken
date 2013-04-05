using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hadouken.Common.Messaging;

namespace Hadouken.Messaging.Msmq
{
    public class SubscribeMessage : Message
    {
        public string Path { get; set; }
    }
}
