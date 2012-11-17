using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Hadouken.BitTorrent;

namespace Hadouken.Data.Models
{
    public class TorrentInfo : Model
    {
        public virtual string InfoHash { get; set; }
        public virtual byte[] Data { get; set; }
        public virtual byte[] FastResumeData { get; set; }
        public virtual long DownloadedBytes { get; set; }
        public virtual long UploadedBytes { get; set; }
        public virtual TorrentState State { get; set; }
        public virtual string SavePath { get; set; }
        public virtual string Label { get; set; }
        public virtual DateTime StartTime { get; set; }
        public virtual DateTime? CompletedTime { get; set; }

        // Settings
        public virtual long ConnectionRetentionFactor { get; set; }
        public virtual bool EnablePeerExchange { get; set; }
        public virtual bool InitialSeedingEnabled { get; set; }
        public virtual int MaxConnections { get; set; }
        public virtual int MaxDownloadSpeed { get; set; }
        public virtual int MaxUploadSpeed { get; set; }
        public virtual int MinimumTimeBetweenReviews { get; set; }
        public virtual int PercentOfMaxRateToSkipReview { get; set; }
        public virtual int UploadSlots { get; set; }
        public virtual bool UseDht { get; set; }
    }
}
