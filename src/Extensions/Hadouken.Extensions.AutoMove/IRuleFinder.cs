using Hadouken.Common.BitTorrent;
using Hadouken.Extensions.AutoMove.Data.Models;

namespace Hadouken.Extensions.AutoMove {
    public interface IRuleFinder {
        /// <summary>
        ///     Finds the first rule that has one or more parameters matching the completed torrent.
        /// </summary>
        /// <returns>A matching rule or null.</returns>
        Rule FindRule(ITorrent torrent);
    }
}