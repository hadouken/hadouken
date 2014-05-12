using System;

namespace Hadouken.Messaging
{
    public abstract class Message
    {
        private readonly Guid _id;

        protected Message()
        {
            _id = Guid.NewGuid();
        }

        public Guid Id
        {
            get { return _id; }
        }
    }
}
