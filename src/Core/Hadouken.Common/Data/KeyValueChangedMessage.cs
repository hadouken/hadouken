using System;
using Hadouken.Common.Messaging;

namespace Hadouken.Common.Data
{
    public sealed class KeyValueChangedMessage : IMessage
    {
        private readonly string[] _keys;

        public KeyValueChangedMessage(string[] keys)
        {
            if (keys == null) throw new ArgumentNullException("keys");
            _keys = keys;
        }

        public string[] Keys
        {
            get { return _keys; }
        }
    }
}
