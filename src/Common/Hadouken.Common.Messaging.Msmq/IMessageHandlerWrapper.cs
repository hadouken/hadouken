using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hadouken.Common.Messaging.Msmq
{
    public interface IMessageHandlerWrapper
    {
        void Execute(Message message);
    }
}
