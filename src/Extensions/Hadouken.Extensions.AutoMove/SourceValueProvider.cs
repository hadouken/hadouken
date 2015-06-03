using Hadouken.Common.BitTorrent;
using Hadouken.Common.Extensibility;
using Hadouken.Extensions.AutoMove.Data.Models;

namespace Hadouken.Extensions.AutoMove {
    [Component]
    public class SourceValueProvider : ISourceValueProvider {
        public string GetValue(ITorrent torrent, ParameterSource source) {
            switch (source) {
                case ParameterSource.Label:
                    return torrent.Label;
                case ParameterSource.Name:
                    return torrent.Name;
            }

            return string.Empty;
        }
    }
}