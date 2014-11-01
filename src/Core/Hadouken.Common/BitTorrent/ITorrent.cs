using System.Collections.Generic;

namespace Hadouken.Common.BitTorrent
{
    public interface ITorrent
    {
        /// <summary>
        /// Gets the calculated info hash for this torrent.
        /// </summary>
        string InfoHash { get; }

        /// <summary>
        /// Gets the name of the torrent as it is provided in the metadata file.
        /// If no metadata file is present, this will be null.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the label assigned to this torrent.
        /// </summary>
        string Label { get; }

        /// <summary>
        /// Gets the total size of the torrent in bytes. If no metadata file is
        /// present, this will be -1.
        /// </summary>
        long Size { get; }

        /// <summary>
        /// Gets the progress for the torrent. This varies depending on state.
        /// </summary>
        float Progress { get; }

        /// <summary>
        /// Gets the save path, eg. where this torrent will save its data.
        /// </summary>
        string SavePath { get; }

        /// <summary>
        /// Gets the current download speed in bytes.
        /// </summary>
        long DownloadSpeed { get; }

        /// <summary>
        /// Gets the current upload speed in bytes.
        /// </summary>
        long UploadSpeed { get; }

        /// <summary>
        /// Gets the downloaded bytes for the torrent in the current session.
        /// </summary>
        long DownloadedBytes { get; }

        /// <summary>
        /// Gets the uploaded bytes for the torrent in the current session.
        /// </summary>
        long UploadedBytes { get; }

        /// <summary>
        /// Gets the total number of downloaded bytes for this torrent.
        /// </summary>
        long TotalDownloadedBytes { get; }

        /// <summary>
        /// Gets the total number of uploaded bytes for this torrent.
        /// </summary>
        long TotalUploadedBytes { get; }

        /// <summary>
        /// Gets the total number of bytes remaining (wanted - done).
        /// </summary>
        long TotalRemainingBytes { get; }

        /// <summary>
        /// Gets the current state for the torrent.
        /// </summary>
        TorrentState State { get; }

        /// <summary>
        /// Gets a value indicating whether this torrent is paused or not.
        /// </summary>
        bool Paused { get; }

        /// <summary>
        /// Gets a value indicating whether this torrent is finished. A torrent
        /// is considered finished if all pieces with a priority > 0 are
        /// downloaded.
        /// </summary>
        bool IsFinished { get; }

        /// <summary>
        /// Gets a value indicating whether this torrent is seeding. <c>True</c>
        /// is all pieces are downloaded.
        /// </summary>
        bool IsSeed { get; }

        /// <summary>
        /// Gets the position of this torrent in the queue.
        /// </summary>
        int QueuePosition { get; }

        /// <summary>
        /// Gets the error message if this torrent is errored. <c>null</c> if no error.
        /// </summary>
        string Error { get; }

        /// <summary>
        /// Gets the torrent-specific settings.
        /// </summary>
        ITorrentSettings GetSettings();

        /// <summary>
        /// Gets a list of the files that make up this torrent.
        /// </summary>
        IEnumerable<ITorrentFile> GetFiles();

        /// <summary>
        /// Gets a list of the file priorities.
        /// </summary>
        /// <returns></returns>
        IEnumerable<int> GetFilePriorities();
            
        /// <summary>
        /// Gets the file progress as a list of floats (0.0 -> 1.0).
        /// </summary>
        IEnumerable<float> GetFileProgress();
            
        /// <summary>
        /// Gets a list of peers for this torrent.
        /// </summary>
        IEnumerable<IPeer> GetPeers();
    }
}
