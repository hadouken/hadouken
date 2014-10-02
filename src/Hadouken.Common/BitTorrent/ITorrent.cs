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
        /// Gets the total number of downloaded bytes for this torrent.
        /// </summary>
        long TotalDownloadedBytes { get; }

        /// <summary>
        /// Gets the total number of uploaded bytes for this torrent.
        /// </summary>
        long TotalUploadedBytes { get; }

        /// <summary>
        /// Gets the current state for the torrent.
        /// </summary>
        TorrentState State { get; }

        /// <summary>
        /// Gets a value indicating whether this torrent is paused or not.
        /// </summary>
        bool Paused { get; }

        /// <summary>
        /// Gets a list of the files that make up this torrent.
        /// </summary>
        ITorrentFile[] Files { get; }

        /// <summary>
        /// Gets a list of peers for this torrent.
        /// </summary>
        IPeer[] Peers { get; }

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
        bool IsSeeding { get; }
    }
}
