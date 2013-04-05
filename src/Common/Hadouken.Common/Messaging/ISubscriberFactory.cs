using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hadouken.Common.Messaging
{
    public interface ISubscriberFactory
    {
        ISubscriber Create(string name);
    }
}
