using System;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Hadouken.Common.Text.BEncoding
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public sealed class BEncodedString : BEncodedValue
    {
        private readonly byte[] _data;

        public BEncodedString(byte[] data)
        {
            if (data == null) throw new ArgumentNullException("data");
            _data = data;
        }

        public static implicit operator string(BEncodedString value)
        {
            return Encoding.UTF8.GetString(value._data);
        }

        public static implicit operator BEncodedString(string value)
        {
            return new BEncodedString(Encoding.UTF8.GetBytes(value));
        }

        public override bool Equals(object obj)
        {
            var other = obj as BEncodedString;
            return other != null && other._data.SequenceEqual(_data);
        }

        public override int GetHashCode()
        {
            return Encoding.UTF8.GetString(_data).GetHashCode();
        }

        private string DebuggerDisplay
        {
            get { return this; }
        }
    }
}
