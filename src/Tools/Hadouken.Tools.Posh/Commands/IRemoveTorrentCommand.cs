using Hadouken.Tools.Posh.Models;

namespace Hadouken.Tools.Posh.Commands {
    public interface IRemoveTorrentCommand : ICommand {
        Torrent Torrent { get; set; }
        bool RemoveData { get; set; }
    }
}