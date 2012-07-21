using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hadouken.BitTorrent;
using MonoTorrent.Client.Tracker;

namespace Hadouken.Impl.BitTorrent
{
    public class HdknTracker : ITracker
    {
        private Tracker _tracker;

        internal HdknTracker(Tracker tracker)
        {
            _tracker = tracker;
        }

        public bool CanAnnounce
        {
            get { return _tracker.CanAnnounce; }
        }

        public bool CanScrape
        {
            get { return _tracker.CanScrape; }
        }

        public int Complete
        {
            get { return _tracker.Complete; }
        }

        public int Downloaded
        {
            get { return _tracker.Downloaded; }
        }

        public string FailureMessage
        {
            get { return _tracker.FailureMessage; }
        }

        public int Incomplete
        {
            get { return _tracker.Incomplete; }
        }

        public TimeSpan MinUpdateInterval
        {
            get { return _tracker.MinUpdateInterval; }
        }

        public TrackerState Status
        {
            get { return (TrackerState)(int)_tracker.Status; }
        }

        public TimeSpan UpdateInterval
        {
            get { return _tracker.UpdateInterval; }
        }

        public Uri Uri
        {
            get { return _tracker.Uri; }
        }

        public string WarningMessage
        {
            get { return _tracker.WarningMessage; }
        }
    }
}
