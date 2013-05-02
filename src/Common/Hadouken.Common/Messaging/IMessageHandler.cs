using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hadouken.Common.Messaging
{
    public interface IMessageHandler<in TMessage> where TMessage : IMessage
    {
        void Handle(TMessage message);
    }
}
