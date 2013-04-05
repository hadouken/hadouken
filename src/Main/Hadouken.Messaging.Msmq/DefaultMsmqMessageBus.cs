using System;
using System.Collections.Generic;
using System.Linq;
using System.Messaging;
using System.Text;
using System.Threading.Tasks;
using Hadouken.Common.Messaging;
using Hadouken.Messaging.Msmq.Formatters;
using Message = Hadouken.Common.Messaging.Message;

namespace Hadouken.Messaging.Msmq
{
    public class MsmqMessageBus : IMsmqMessageBus
    {
        private readonly string _queuePath;
        private MessageQueue _queue;

        private readonly IDictionary<string, MessageQueue> _subscribers =
            new Dictionary<string, MessageQueue>(StringComparer.InvariantCultureIgnoreCase); 

        public MsmqMessageBus(string queuePath, params MessageQueue[] subscribers)
        {
            if(queuePath == null)
                throw new ArgumentNullException("queuePath");


            _queuePath = queuePath;

            if (subscribers == null)
                return;

            foreach (var subscriber in subscribers)
            {
                _subscribers.Add(subscriber.Path, subscriber);
            }
        }

        public IDictionary<string, MessageQueue> ExternalQueues
        {
            get { return _subscribers; }
        } 

        private void BeginReceive(IAsyncResult asyncResult)
        {
            var msg = _queue.EndReceive(asyncResult);
            _queue.BeginReceive(TimeSpan.MaxValue, null, BeginReceive);

            Task.Factory.StartNew(() => ParseIncomingMessage(msg));
        }

        private void ParseIncomingMessage(System.Messaging.Message message)
        {
            var obj = message.Body;
        }

        public void Load()
        {
            if (!MessageQueue.Exists(_queuePath))
                MessageQueue.Create(_queuePath);

            _queue = new MessageQueue(_queuePath) { Formatter = new JsonMessageFormatter() };
            _queue.BeginReceive(TimeSpan.MaxValue, null, BeginReceive);

            Publish(new SubscribeMessage {Path = _queuePath});
        }

        public void Unload()
        {
            Publish(new UnsubscribeMessage {Path = _queuePath});

            _queue.Dispose();

            if (MessageQueue.Exists(_queuePath))
                MessageQueue.Delete(_queuePath);
        }

        public void Publish<TMessage>(TMessage message) where TMessage : Message
        {
            var wrapper = new MsmqMessage<TMessage>()
                {
                    Message = message,
                    TypeName = typeof (TMessage).FullName
                };
            
            foreach (var subscriber in _subscribers.Values)
            {
                subscriber.Send(wrapper);
            }
        }

        public void Subscribe<TMessage>(Action<TMessage> callback) where TMessage : Message
        {
            throw new NotImplementedException();
        }
    }
}
