using Hadouken.Common.BitTorrent;
using Hadouken.Extensions.AutoMove.Data.Models;

namespace Hadouken.Extensions.AutoMove {
    public interface ISourceValueProvider {
        string GetValue(ITorrent torrent, ParameterSource source);
    }
}