using System.Collections.Generic;

namespace Hadouken.Common.BitTorrent
{
    public interface ITorrentEngine
    {
        /// <summary>
        /// Gets a list of all torrents registered with this <see cref="ITorrentEngine"/> instance.
        /// </summary>
        IEnumerable<ITorrent> GetAll();

        /// <summary>
        /// Gets a single <see cref="ITorrent"/> by its InfoHash property.
        /// </summary>
        ITorrent GetByInfoHash(string infoHash);

        /// <summary>
        /// Gets a list of all labels.
        /// </summary>
        IEnumerable<string> GetLabels();
    }
}
