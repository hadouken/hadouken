using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hadouken.Messaging;

namespace Hadouken.Messages
{
    public interface ISettingChanged : IMessage
    {
        string Key { get; set; }
        string OldValue { get; set; }
        string NewValue { get; set; }
    }
}
