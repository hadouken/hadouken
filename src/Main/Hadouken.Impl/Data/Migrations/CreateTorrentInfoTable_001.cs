using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Migrator;
using Migrator.Framework;

namespace Hadouken.Impl.Data.Migrations
{
    [Migration(201206251243)]
    public class CreateTorrentInfoTable_001 : Migration
    {
        public override void Up()
        {
            Database.AddTable("TorrentInfo",
                new Column("Id", System.Data.DbType.Int32, ColumnProperty.PrimaryKeyWithIdentity),
                new Column("InfoHash", System.Data.DbType.String, 50),
                new Column("Data", System.Data.DbType.Binary),
                new Column("FastResumeData", System.Data.DbType.Binary),
                new Column("DownloadedBytes", System.Data.DbType.Int64),
                new Column("UploadedBytes", System.Data.DbType.Int64),
                new Column("State", System.Data.DbType.String, 50),
                new Column("SavePath", System.Data.DbType.String),
                new Column("Label", System.Data.DbType.String)
            );
        }

        public override void Down()
        {
            Database.RemoveTable("TorrentInfo");
        }
    }
}
