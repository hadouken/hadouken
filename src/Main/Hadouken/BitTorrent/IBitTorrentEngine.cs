using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hadouken.BitTorrent
{
    public interface IBitTorrentEngine
    {
        void Load();
        void Unload();

        IDictionary<string, ITorrentManager> Managers { get; }

        void StartAll();
        void StopAll();

        /// <summary>
        /// Creates a new ITorrentManager based on the magnet link. Registers it to the engine and adds it in a stopped state.
        /// </summary>
        /// <param name="url">The magnet link URL.</param>
        /// <returns>An ITorrentManager instance representing the successfully created manager, or null if creation failed.</returns>
        ITorrentManager AddMagnetLink(string url);

        /// <summary>
        /// Creates a new ITorrentManager based on the magnet link. Registers it to the engine and adds it in a stopped state.
        /// </summary>
        /// <param name="url">The magnet link URL.</param>
        /// <param name="savePath">Where on the disk to save the downloaded data. Can point to existing (uncomplete or complete) data.</param>
        /// <returns>An ITorrentManager instance representing the successfully created manager, or null if creation failed.</returns>
        ITorrentManager AddMagnetLink(string url, string savePath);

        /// <summary>
        /// Creates a new ITorrentManager based on the given data. Registers it to the engine and adds it in a stopped state.
        /// </summary>
        /// <param name="torrentData">The byte array containing the torrent file data.</param>
        /// <returns>An ITorrentManager instance representing the successfully created manager, or null if creation failed.</returns>
        ITorrentManager AddTorrent(byte[] torrentData);

        /// <summary>
        /// Creates a new ITorrentManager based on the gived data. Registers it to the engine and adds it in a stopped state.
        /// </summary>
        /// <param name="torrentData">The byte array containing the torrent file data.</param>
        /// <param name="savePath">Where on the disk to save the downloaded data. Can point to existing (uncomplete or complete) data.</param>
        /// <returns>An ITorrentManager instance representing the successfully created manager, or null if creation failed.</returns>
        ITorrentManager AddTorrent(byte[] torrentData, string savePath);
        
        /// <summary>
        /// Remove and unregister the given manager from the engine.
        /// </summary>
        /// <param name="manager">The manager to remove.</param>
        void RemoveTorrent(ITorrentManager manager);
    }
}
