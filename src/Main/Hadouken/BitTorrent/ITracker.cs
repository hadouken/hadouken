using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hadouken.BitTorrent
{
    public interface ITracker
    {
        bool CanAnnounce { get; }
        bool CanScrape { get; }
        int Complete { get; }
        int Downloaded { get; }
        string FailureMessage { get; }
        int Incomplete { get; }
        TimeSpan MinUpdateInterval { get; }
        TrackerState Status { get; }
        TimeSpan UpdateInterval { get; }
        Uri Uri { get; }
        string WarningMessage { get; }
    }
}
