using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hadouken.BitTorrent;
using MonoTorrent.Client;

namespace Hadouken.Impl.BitTorrent
{
    public class HdknTorrentSettings : ITorrentSettings
    {
        private TorrentSettings _settings;

        internal HdknTorrentSettings(TorrentSettings settings)
        {
            _settings = settings;
        }

        public long ConnectionRetentionFactor
        {
            get { return _settings.ConnectionRetentionFactor; }
            set { _settings.ConnectionRetentionFactor = value; }
        }

        public bool EnablePeerExchange
        {
            get { return _settings.EnablePeerExchange; }
            set { _settings.EnablePeerExchange = value; }
        }

        public bool InitialSeedingEnabled
        {
            get { return _settings.InitialSeedingEnabled; }
            set { _settings.InitialSeedingEnabled = value; }
        }

        public int MaxConnections
        {
            get { return _settings.MaxConnections; }
            set { _settings.MaxConnections = value; }
        }

        public int MaxDownloadSpeed
        {
            get { return _settings.MaxDownloadSpeed; }
            set { _settings.MaxDownloadSpeed = value; }
        }

        public int MaxUploadSpeed
        {
            get { return _settings.MaxUploadSpeed; }
            set { _settings.MaxUploadSpeed = value; }
        }

        public int MinimumTimeBetweenReviews
        {
            get { return _settings.MinimumTimeBetweenReviews; }
            set { _settings.MinimumTimeBetweenReviews = value; }
        }

        public int PercentOfMaxRateToSkipReview
        {
            get { return _settings.PercentOfMaxRateToSkipReview; }
            set { _settings.PercentOfMaxRateToSkipReview = value; }
        }

        public TimeSpan TimeToWaitUntilIdle
        {
            get { return _settings.TimeToWaitUntilIdle; }
            set { _settings.TimeToWaitUntilIdle = value; }
        }

        public int UploadSlots
        {
            get { return _settings.UploadSlots; }
            set { _settings.UploadSlots = value; }
        }

        public bool UseDht
        {
            get { return _settings.UseDht; }
            set { _settings.UseDht = value; }
        }
    }
}
