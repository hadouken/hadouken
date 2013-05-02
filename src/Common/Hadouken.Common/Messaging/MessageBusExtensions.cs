using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Hadouken.Common.Messaging
{
    public static class MessageBusExtensions
    {
        /// <summary>
        /// Sends a message on the bus and waits for a response to this message of the specified message type.
        /// </summary>
        /// <typeparam name="TMessage">A class that inherits Message.</typeparam>
        /// <param name="messageBus"></param>
        /// <param name="outgoing">The message to send.</param>
        /// <returns>The message which is a response to the message sent.</returns>
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
