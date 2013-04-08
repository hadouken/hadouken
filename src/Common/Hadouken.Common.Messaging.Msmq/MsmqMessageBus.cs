using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using MassTransit;
using NLog;

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

                b.Subscribe(s => s.Handler<Message>(OnMessage));
            });

            Logger.Info("Created message queue '{0}'.", _queuePath);
        }

        public void Unload()
        {
        }

        private void OnMessage(Message message)
        {
            if(message == null)
                throw new ArgumentNullException("message");

            var handlerType = typeof (IMessageHandler<>).MakeGenericType(message.GetType());

            // Resolve all types of the above handlerType

            // Union this with all ad-hoc listeners (Action<T> subscribers)

            // Invoke each and every last one of them
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
