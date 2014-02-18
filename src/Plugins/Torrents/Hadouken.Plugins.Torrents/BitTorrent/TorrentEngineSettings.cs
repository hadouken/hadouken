namespace Hadouken.Plugins.Torrents.BitTorrent
{
    public class TorrentEngineSettings
    {
        public string SavePath { get; set; }

        public int ListenPort { get; set; }

        public int GlobalMaxConnections { get; set; }

        public int GlobalMaxDownloadSpeed { get; set; }

        public int GlobalMaxHalfOpenConnections { get; set; }

        public int GlobalMaxUploadSpeed { get; set; }
    }
}
