using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hadouken.BitTorrent
{
    public interface ITorrentManager
    {
        IBitField BitField { get; }
        bool CanUseDht { get; }
        bool Complete { get; }
        bool HashChecked { get; }
        int HashFails { get; }
        bool HasMetadata { get; }
        List<Uri> InactivePeerList { get; }
        int InactivePeers { get; }
        string InfoHash { get; }
        bool IsInEndGame { get; }
        bool IsInitialSeeding { get; }
        int OpenConnections { get; }
        int PeerReviewRoundsComplete { get; }
        double Progress { get; }
        string SavePath { get; }
        DateTime StartTime { get; }
        DateTime? CompletedTime { get; }
        TorrentState State { get; }
        int UploadingTo { get; }
        TimeSpan ETA { get; }
        string Label { get; set; }

        long DownloadedBytes { get; }
        long UploadedBytes { get; }
        long DownloadSpeed { get; }
        long UploadSpeed { get; }
        long RemainingBytes { get; }
        
        ITorrent Torrent { get; }
        byte[] TorrentData { get; }
        IPeer[] Peers { get; }
        ITracker[] Trackers { get; }

        void Start();
        void Stop();
        void Pause();
        void Move(string newLocation);
        void HashCheck(bool autoStart);

        void LoadFastResume(byte[] data);
        byte[] SaveFastResume();
    }
}
