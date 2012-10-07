using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hadouken.BitTorrent
{
    public interface ITorrentSettings
    {
        long ConnectionRetentionFactor { get; set; }
        bool EnablePeerExchange { get; set; }
        bool InitialSeedingEnabled { get; set; }
        int MaxConnections { get; set; }
        int MaxDownloadSpeed { get; set; }
        int MaxUploadSpeed { get; set; }
        int MinimumTimeBetweenReviews { get; set; }
        int PercentOfMaxRateToSkipReview { get; set; }
        TimeSpan TimeToWaitUntilIdle { get; set; }
        int UploadSlots { get; set; }
        bool UseDht { get; set; }
    }
}
