using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hadouken.Common.Messaging
{
    public interface IMessage
    {
        Guid Id { get; }

        Guid SourceId { get; set; }
    }
}
