using System;
using System.Collections.Generic;
using System.Linq;
using Hadouken.Common.Data;

namespace Hadouken.Core.BitTorrent.Data {
    public sealed class TorrentMetadataRepository : ITorrentMetadataRepository {
        private readonly IDbConnection _dbConnection;

        public TorrentMetadataRepository(IDbConnection dbConnection) {
            if (dbConnection == null) {
                throw new ArgumentNullException("dbConnection");
            }
            this._dbConnection = dbConnection;
        }

        public void SetLabel(string infoHash, string label) {
            var param = new {Label = label, InfoHash = infoHash};

            const string countQuery = "select count(*) from TorrentMetadata tm where tm.InfoHash = @InfoHash";
            var count = this._dbConnection.Query<int>(countQuery, param).Single();

            if (count > 0) {
                const string updateQuery = "update TorrentMetadata set Label = @Label where InfoHash = @InfoHash";
                this._dbConnection.Execute(updateQuery, param);
            }
            else {
                const string insertQuery = "insert into TorrentMetadata (InfoHash, Label) values (@InfoHash, @Label);";
                this._dbConnection.Execute(insertQuery, param);
            }
        }

        public string GetLabel(string infoHash) {
            const string query = "select tm.Label from TorrentMetadata tm where tm.InfoHash = @InfoHash";
            return this._dbConnection.Query<string>(query, new {InfoHash = infoHash}).SingleOrDefault();
        }

        public IEnumerable<string> GetAllLabels() {
            const string query = "select distinct tm.Label from TorrentMetadata tm where tm.Label is not null and trim(tm.Label) <> ''";
            return this._dbConnection.Query<string>(query);
        }
    }
}