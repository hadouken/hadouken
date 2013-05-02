using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Magnum.Reflection;

using MassTransit;
using MassTransit.SubscriptionConfigurators;
using MassTransit.Util;

namespace Hadouken.Common.Messaging.Msmq
{
    internal class ConsumerFactoryConfigurator
    {
        private readonly SubscriptionBusServiceConfigurator _configurator;

        public ConsumerFactoryConfigurator(SubscriptionBusServiceConfigurator configurator)
        {
            _configurator = configurator;
        }

        public void ConfigureConsumer(Type type)
        {
            this.FastInvoke(new[] {type}, "Configure");
        }

        [UsedImplicitly]
        public void Configure<T>() where T : class, IMessage
        {
            _configurator.Handler(new Action<T>(GetHandlers));
        }

        private void GetHandlers<T>(T message) where T : IMessage
        {
            var handlers = Kernel.GetAll<IMessageHandler<T>>();

            foreach (var handler in handlers)
            {
                handler.Handle(message);
            }
        }
    }
}
