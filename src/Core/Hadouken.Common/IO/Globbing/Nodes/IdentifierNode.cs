///////////////////////////////////////////////////////////////////////
// Portions of this code was ported from glob-js by Kevin Thompson.
// https://github.com/kthompson/glob-js
///////////////////////////////////////////////////////////////////////

namespace Hadouken.Common.IO.Globbing.Nodes {
    internal sealed class IdentifierNode : Node {
        private readonly string _identifier;

        public IdentifierNode(string identifier) {
            this._identifier = identifier;
        }

        public override bool IsWildcard {
            get { return false; }
        }

        public string Identifier {
            get { return this._identifier; }
        }

        public override string Render() {
            return this._identifier;
        }
    }
}