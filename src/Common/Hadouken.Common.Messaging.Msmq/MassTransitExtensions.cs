using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MassTransit;
using MassTransit.SubscriptionConfigurators;

namespace Hadouken.Common.Messaging.Msmq
{
    public static class MassTransitExtensions
    {
        public static void LoadHandlers(this SubscriptionBusServiceConfigurator configurator)
        {
            // Find all concrete message types
            var messageTypes = (from asm in AppDomain.CurrentDomain.GetAssemblies()
                                from type in asm.GetTypes()
                                where typeof (IMessage).IsAssignableFrom(type)
                                where type.IsClass && !type.IsAbstract
                                select type);

            foreach (var messageType in messageTypes)
            {
                var messageConfigurator = new ConsumerFactoryConfigurator(configurator);
                messageConfigurator.ConfigureConsumer(messageType);
            }
        }
    }
}
