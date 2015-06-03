using System;
using System.Collections.Generic;
using System.Linq;
using Hadouken.Common.Logging;

namespace Hadouken.Common.Messaging {
    public class MessageBus : IMessageBus {
        private readonly IDictionary<Type, IList<object>> _callbacks;
        private readonly Func<IEnumerable<IMessageHandler>> _handlers;
        private readonly ILogger<MessageBus> _logger;

        public MessageBus(ILogger<MessageBus> logger, Func<IEnumerable<IMessageHandler>> handlers) {
            if (logger == null) {
                throw new ArgumentNullException("logger");
            }
            this._logger = logger;
            this._handlers = handlers;
            this._callbacks = new Dictionary<Type, IList<object>>();
        }

        public void Publish<T>(T message) where T : class, IMessage {
            if (message == null) {
                throw new ArgumentNullException("message");
            }

            var t = typeof (T);

            if (this._callbacks.ContainsKey(t)) {
                var callbacks = this._callbacks[t].OfType<Action<T>>().ToList();

                this._logger.Debug(string.Format("Sending {0} to {1} callbacks.", t.Name, callbacks.Count));

                foreach (var callback in callbacks) {
                    try {
                        callback(message);
                    }
                    catch (Exception e) {
                        this._logger.Error(e, "Error when invoking callback");
                    }
                }
            }

            // Find handlers
            var handlerType = typeof (IMessageHandler<T>);
            var handlers = (from handler in this._handlers()
                let type = handler.GetType()
                where handlerType.IsAssignableFrom(type)
                select (IMessageHandler<T>) handler).ToList();

            this._logger.Debug(string.Format("Sending {0} to {1} handlers.", t.Name, handlers.Count));

            foreach (var handler in handlers) {
                handler.Handle(message);
            }
        }

        public void Subscribe<T>(Action<T> callback) where T : IMessage {
            var t = typeof (T);

            // Add empty list
            if (!this._callbacks.ContainsKey(t)) {
                this._callbacks.Add(t, new List<object>());
            }

            // Add callback to list
            this._callbacks[t].Add(callback);
        }

        public void Unsubscribe<T>(Action<T> callback) where T : IMessage {
            var t = typeof (T);

            if (!this._callbacks.ContainsKey(t)) {
                return;
            }

            this._callbacks[t].Remove(callback);
        }
    }
}