namespace Hadouken.Core.BitTorrent.Data
{
    public interface ITorrentMetadataRepository
    {
        void SetLabel(string infoHash, string label);

        string GetLabel(string infoHash);
    }
}
