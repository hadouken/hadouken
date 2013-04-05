using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hadouken.Common.Messaging
{
    public interface IMessageHandler<in TMessage> where TMessage : Message
    {
        void Handle(TMessage message);
    }
}
