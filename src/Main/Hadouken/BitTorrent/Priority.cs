using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hadouken.BitTorrent
{
    public enum Priority
    {
        DoNotDownload = 0,
        Lowest = 1,
        Low = 2,
        Normal = 4,
        High = 8,
        Highest = 16,
        Immediate = 32,
    }
}
