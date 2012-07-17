using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hadouken.BitTorrent
{
    [Flags]
    public enum EncryptionTypes
    {
        None = 0,
        PlainText = 1,
        RC4Header = 2,
        RC4Full = 4,
        All = 7,
    }
}
