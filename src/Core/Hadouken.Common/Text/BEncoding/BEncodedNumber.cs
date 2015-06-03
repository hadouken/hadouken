using System.Diagnostics;

namespace Hadouken.Common.Text.BEncoding {
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public sealed class BEncodedNumber : BEncodedValue {
        private readonly long _value;

        public BEncodedNumber(long value) {
            this._value = value;
        }

        private string DebuggerDisplay {
            get { return string.Format("{0}", (long) this); }
        }

        public static implicit operator long(BEncodedNumber number) {
            return number._value;
        }

        public static implicit operator BEncodedNumber(long value) {
            return new BEncodedNumber(value);
        }
    }
}