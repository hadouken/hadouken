using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using MassTransit;

namespace Hadouken.Common.Messaging.Msmq
{
    public class TestMessage
    {
        public string Text { get; set; }
    }

    public class MsmqMessageBus : IMsmqMessageBus
    {
        private readonly string _queuePath;
        private IServiceBus _serviceBus;

        public MsmqMessageBus(string queuePath)
        {
            if(queuePath == null)
                throw new ArgumentNullException("queuePath");

            _queuePath = queuePath;
        }

        private void OnMessage(TestMessage message)
        {
            //
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

                b.Subscribe(s => s.Handler<TestMessage>(OnMessage));
            });
        }

        public void Unload()
        {
        }

        public void Publish<TMessage>(TMessage message) where TMessage : Message
        {
            //
        }

        public void Subscribe<TMessage>(Action<TMessage> callback) where TMessage : Message
        {
            //
        }
    }
}
