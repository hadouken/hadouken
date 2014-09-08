///////////////////////////////////////////////////////////////////////
// Portions of this code was ported from glob-js by Kevin Thompson.
// https://github.com/kthompson/glob-js
///////////////////////////////////////////////////////////////////////

namespace Hadouken.Common.IO.Globbing
{
    internal abstract class Node
    {
        public abstract bool IsWildcard { get; }

        public abstract string Render();
    }
}
