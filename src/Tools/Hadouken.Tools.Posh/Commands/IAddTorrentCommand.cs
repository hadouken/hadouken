namespace Hadouken.Tools.Posh.Commands {
    public interface IAddTorrentCommand : ICommand {
        string Path { get; set; }
        string SavePath { get; set; }
    }
}