using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hadouken.BitTorrent
{
    public interface IBitTorrentEngine : IComponent
    {
        void Load();
        void Unload();

        IDictionary<string, ITorrentManager> Managers { get; }

        void StartAll();
        void StopAll();

        ITorrentManager CreateManager(byte[] torrentData);

        /// <summary>
        /// Creates a new ITorrentManager based on the gived data. Registers it to the engine and adds it in a stopped state.
        /// </summary>
        /// <param name="torrentData">The byte array defining the torrent file contents.</param>
        /// <param name="savePath">Where on the disk to save the downloaded data. Can point to existing (uncomplete or complete) data.</param>
        /// <returns>A ITorrentManager instance representing the successfully created manager, or null if creation failed.</returns>
        ITorrentManager CreateManager(byte[] torrentData, string savePath);
        
        /// <summary>
        /// Remove and unregister the given manager from the engine.
        /// </summary>
        /// <param name="manager">The manager to remove.</param>
        void RemoveManager(ITorrentManager manager);
    }
}
