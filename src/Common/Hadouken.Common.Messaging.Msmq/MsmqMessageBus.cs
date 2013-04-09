using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using MassTransit;
using MassTransit.SubscriptionConfigurators;
using NLog;
using MassTransit.BusConfigurators;

namespace Hadouken.Common.Messaging.Msmq
{
    public class MsmqMessageBus : IMsmqMessageBus
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly string _queuePath;
        private IServiceBus _serviceBus;

        public MsmqMessageBus(string queuePath)
        {
            if(queuePath == null)
                throw new ArgumentNullException("queuePath");

            _queuePath = queuePath;
        }

        public void Load()
        {
            _serviceBus = ServiceBusFactory.New(b =>
            {
                b.UseMsmq();
                b.VerifyMsmqConfiguration();
                b.UseMulticastSubscriptionClient();
                b.ReceiveFrom(_queuePath);
                b.SetCreateMissingQueues(true);
                b.SetPurgeOnStartup(true);

                b.Subscribe(sbc => sbc.LoadHandlers());
            });

            Logger.Info("Created message queue '{0}'.", _queuePath);
        }

        public void Unload()
        {
            _serviceBus.Dispose();
        }

        public void Publish<TMessage>(TMessage message) where TMessage : Message
        {
            _serviceBus.Publish(message);
        }

        public void Subscribe<TMessage>(Action<TMessage> callback) where TMessage : Message
        {
            //
        }
    }
}
