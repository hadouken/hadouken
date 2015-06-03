///////////////////////////////////////////////////////////////////////
// Portions of this code was ported from glob-js by Kevin Thompson.
// https://github.com/kthompson/glob-js
///////////////////////////////////////////////////////////////////////

namespace Hadouken.Common.IO.Globbing {
    internal sealed class Token {
        private readonly TokenKind _kind;
        private readonly string _value;

        public Token(TokenKind kind, string value) {
            this._kind = kind;
            this._value = value;
        }

        public TokenKind Kind {
            get { return this._kind; }
        }

        public string Value {
            get { return this._value; }
        }
    }
}