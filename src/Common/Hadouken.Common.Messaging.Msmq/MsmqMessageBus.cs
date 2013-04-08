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

        private readonly IDictionary<Type, IMessageHandlerWrapper> _wrapperCache =
            new Dictionary<Type, IMessageHandlerWrapper>(); 

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
            _serviceBus.Dispose();
        }

        private void OnMessage(Message message)
        {
            if(message == null)
                throw new ArgumentNullException("message");

            var handlerType = typeof (IMessageHandler<>).MakeGenericType(message.GetType());
            var wrapperType = typeof (MessageHandlerWrapper<>).MakeGenericType(handlerType);
            var handlers = Kernel.GetAll(handlerType);

            foreach (var handler in handlers)
            {
                IMessageHandlerWrapper wrapper;

                if (_wrapperCache.ContainsKey(wrapperType))
                {
                    wrapper = _wrapperCache[wrapperType];
                }
                else
                {
                    wrapper = (IMessageHandlerWrapper)Activator.CreateInstance(wrapperType, handler);
                    _wrapperCache.Add(wrapperType, wrapper);
                }

                if (wrapper == null)
                    throw new InvalidOperationException("No wrapper found.");

                wrapper.Execute(message);
            }

            // Invoke each and every last one of them
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
