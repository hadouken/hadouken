using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hadouken.Common.Messaging
{
    public abstract class Message : IMessage
    {
        protected Message()
        {
            Id = Guid.NewGuid();
        }

        public Guid Id { get; protected set; }

        public Guid SourceId { get; set; }
    }
}
