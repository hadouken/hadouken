using System;
using System.Collections.Generic;
using System.Linq;
using Hadouken.Common.Logging;

namespace Hadouken.Common.Messaging
{
    public class MessageBus : IMessageBus
    {
        private readonly ILogger<MessageBus> _logger;
        private readonly Func<IEnumerable<IMessageHandler>> _handlers;
        private readonly IDictionary<Type, IList<object>> _callbacks;

        public MessageBus(ILogger<MessageBus> logger, Func<IEnumerable<IMessageHandler>> handlers)
        {
            if (logger == null) throw new ArgumentNullException("logger");
            _logger = logger;
            _handlers = handlers;
            _callbacks = new Dictionary<Type, IList<object>>();
        }

        public void Publish<T>(T message) where T : class, IMessage
        {
            if (message == null) throw new ArgumentNullException("message");

            var t = typeof (T);

            if (_callbacks.ContainsKey(t))
            {
                var callbacks = _callbacks[t].OfType<Action<T>>().ToList();

                _logger.Debug(string.Format("Sending {0} to {1} callbacks.", t.Name, callbacks.Count));

                foreach (var callback in callbacks)
                {
                    try
                    {
                        callback(message);
                    }
                    catch (Exception e)
                    {
                        _logger.Error(e, "Error when invoking callback");
                    }
                }
            }

            // Find handlers
            var handlerType = typeof (IMessageHandler<T>);
            var handlers = (from handler in _handlers()
                let type = handler.GetType()
                where handlerType.IsAssignableFrom(type)
                select (IMessageHandler<T>) handler).ToList();

            _logger.Debug(string.Format("Sending {0} to {1} handlers.", t.Name, handlers.Count));

            foreach (var handler in handlers)
            {
                handler.Handle(message);
            }
        }

        public void Subscribe<T>(Action<T> callback) where T : IMessage
        {
            var t = typeof (T);

            // Add empty list
            if (!_callbacks.ContainsKey(t)) _callbacks.Add(t, new List<object>());

            // Add callback to list
            _callbacks[t].Add(callback);
        }

        public void Unsubscribe<T>(Action<T> callback) where T : IMessage
        {
            throw new NotImplementedException();
        }
    }
}