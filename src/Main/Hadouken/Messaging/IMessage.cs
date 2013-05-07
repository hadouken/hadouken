using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hadouken.Messaging
{
    public interface IMessage
    {
        Guid Id { get; set; }
    }
}
