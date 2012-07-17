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
        TimeSpan MinUpdateInterval { get; }
        TrackerState Status { get; }
        Uri Uri { get; }
        TimeSpan UpdateInterval { get; }
        string WarningMessage { get; }
    }
}
