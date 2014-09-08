using System;

namespace Hadouken.Common.Messaging
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class MessageAttribute : Attribute
    {
        private readonly string _messageId;

        public MessageAttribute(string messageId)
        {
            if (messageId == null) throw new ArgumentNullException("messageId");
            _messageId = messageId;
        }

        public string MessageId
        {
            get { return _messageId; }
        }

        public string Name { get; set; }

        public string Description { get; set; }
    }
}
