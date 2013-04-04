using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Hadouken.Common.Messaging
{
    public static class MessageBusExtensions
    {
        public static TMessage Call<TMessage>(this IMessageBus messageBus, Message outgoing)
            where TMessage : Message
        {
            TMessage result = null;

            var id = outgoing.Id;
            var reset = new ManualResetEvent(false);

            messageBus.Subscribe<TMessage>(_ =>
                {
                    if (id != _.SourceId)
                        return;

                    result = _;
                    reset.Set();
                });

            reset.WaitOne(5000);

            return result;
        }
    }
}
