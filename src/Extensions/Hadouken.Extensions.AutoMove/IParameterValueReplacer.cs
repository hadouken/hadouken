using System.Collections.Generic;
using Hadouken.Common.BitTorrent;
using Hadouken.Extensions.AutoMove.Data.Models;

namespace Hadouken.Extensions.AutoMove {
    public interface IParameterValueReplacer {
        string Replace(ITorrent torrent, IEnumerable<Parameter> parameters, string targetPath);
    }
}