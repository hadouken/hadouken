using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Messaging;

namespace Hadouken.Messaging.Msmq.Formatters
{
    public class JsonMessageFormatter : IMessageFormatter
    {
        public bool CanRead(Message message)
        {
            throw new NotImplementedException();
        }

        public object Read(Message message)
        {
            throw new NotImplementedException();
        }

        public void Write(Message message, object obj)
        {
            throw new NotImplementedException();
        }

        public object Clone()
        {
            throw new NotImplementedException();
        }
    }
}
