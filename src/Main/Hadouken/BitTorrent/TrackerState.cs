using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hadouken.BitTorrent
{
    public enum TrackerState
    {
        Ok = 0,
        Offline = 1,
        InvalidResponse = 2,
    }
}
