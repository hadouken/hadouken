﻿///////////////////////////////////////////////////////////////////////
// Portions of this code was ported from glob-js by Kevin Thompson.
// https://github.com/kthompson/glob-js
///////////////////////////////////////////////////////////////////////

namespace Hadouken.Common.IO.Globbing.Nodes.Roots
{
    internal sealed class UnixRoot : RootNode
    {
        public override bool IsWildcard
        {
            get { return false; }
        }

        public override string Render()
        {
            return string.Empty;
        } 
    }
}