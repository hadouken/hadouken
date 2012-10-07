using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Migrator.Framework;

namespace Hadouken.Impl.Data.Migrations.Alter
{
    [Migration(201210071841)]
    public class AlterTorrentInfoTable_001 : Migration
    {
        public override void Down()
        {
        }

        public override void Up()
        {
            Database.AddColumn("TorrentInfo", new Column("ConnectionRetentionFactor", DbType.Int64));
            Database.AddColumn("TorrentInfo", new Column("EnablePeerExchange", DbType.Byte));
            Database.AddColumn("TorrentInfo", new Column("InitialSeedingEnabled", DbType.Byte));
            Database.AddColumn("TorrentInfo", new Column("MaxConnections", DbType.Int32));
            Database.AddColumn("TorrentInfo", new Column("MaxDownloadSpeed", DbType.Int32));
            Database.AddColumn("TorrentInfo", new Column("MaxUploadSpeed", DbType.Int32));
            Database.AddColumn("TorrentInfo", new Column("MinimumTimeBetweenReviews", DbType.Int32));
            Database.AddColumn("TorrentInfo", new Column("PercentOfMaxRateToSkipReview", DbType.Int32));
            //Database.AddColumn("TorrentInfo", new Column("TimeToWaitUntilIdle", DbType.Time));
            Database.AddColumn("TorrentInfo", new Column("UploadSlots", DbType.Int32, ColumnProperty.NotNull, 4));
            Database.AddColumn("TorrentInfo", new Column("UseDht", DbType.Byte));
        }
    }
}
