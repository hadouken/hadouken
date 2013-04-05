using System;
using System.Messaging;

namespace Hadouken.Common.Messaging.Msmq.Formatters
{
    public class JsonMessageFormatter : IMessageFormatter
    {
        public bool CanRead(System.Messaging.Message message)
        {
            throw new NotImplementedException();
        }

        public object Read(System.Messaging.Message message)
        {
            throw new NotImplementedException();
        }

        public void Write(System.Messaging.Message message, object obj)
        {
            throw new NotImplementedException();
        }

        public object Clone()
        {
            throw new NotImplementedException();
        }
    }
}
