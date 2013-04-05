using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hadouken.Common.Messaging;

namespace Hadouken.Common
{
    public abstract class Plugin
    {
        private readonly IMessageBus _messageBus;
        
        protected Plugin(IMessageBus messageBus)
        {
            if(messageBus == null)
                throw new ArgumentNullException("messageBus");

            _messageBus = messageBus;
        }

        /// <summary>
        /// Gets the MessageBus to use for publishing messages/subscribing to messages.
        /// </summary>
        protected IMessageBus MessageBus
        {
            get { return _messageBus; }
        }

        public abstract void Load();

        public virtual void Unload()
        {
        }
    }
}
