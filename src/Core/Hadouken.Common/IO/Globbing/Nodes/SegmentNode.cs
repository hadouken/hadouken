///////////////////////////////////////////////////////////////////////
// Portions of this code was ported from glob-js by Kevin Thompson.
// https://github.com/kthompson/glob-js
///////////////////////////////////////////////////////////////////////

using System.Collections.Generic;
using System.Linq;

namespace Hadouken.Common.IO.Globbing.Nodes {
    internal sealed class SegmentNode : Node {
        private readonly List<Node> _items;

        public SegmentNode(List<Node> items) {
            this._items = items;
        }

        public override bool IsWildcard {
            get { return this._items.Any(x => x.IsWildcard); }
        }

        public override string Render() {
            return string.Join(string.Empty, this._items.Select(x => x.Render()));
        }
    }
}