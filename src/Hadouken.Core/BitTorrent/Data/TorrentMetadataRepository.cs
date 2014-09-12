using System;
using System.Linq;
using Hadouken.Common.Data;

namespace Hadouken.Core.BitTorrent.Data
{
    public sealed class TorrentMetadataRepository : ITorrentMetadataRepository
    {
        private readonly IDbConnection _dbConnection;

        public TorrentMetadataRepository(IDbConnection dbConnection)
        {
            if (dbConnection == null) throw new ArgumentNullException("dbConnection");
            _dbConnection = dbConnection;
        }

        public void SetLabel(string infoHash, string label)
        {
            var param = new {Label = label, InfoHash = infoHash};

            var countQuery = "select count(*) from TorrentMetadata tm where tm.InfoHash = @InfoHash";
            var count = _dbConnection.Query<int>(countQuery, param).Single();

            if (count > 0)
            {
                var updateQuery = "update TorrentMetadata set Label = @Label where InfoHash = @InfoHash";
                _dbConnection.Execute(updateQuery, param);
            }
            else
            {
                var insertQuery = "insert into TorrentMetadata (InfoHash, Label) values (@InfoHash, @Label);";
                _dbConnection.Execute(insertQuery, param);
            }
        }

        public string GetLabel(string infoHash)
        {
            var query = "select tm.Label from TorrentMetadata tm where tm.InfoHash = @InfoHash";
            return _dbConnection.Query<string>(query, new {InfoHash = infoHash}).SingleOrDefault();
        }
    }
}