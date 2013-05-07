using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Migrator.Framework;

namespace HdknPlugins.Rss.Data.Migrations.Create
{
    [Migration(20121208193410)]
    public class CreateFeedTable_001 : Migration
    {
        public override void Down()
        {
            Database.RemoveTable("plugin_Rss_Feeds");
        }

        public override void Up()
        {
            Database.AddTable("plugin_Rss_Feeds",
                new Column("Id", DbType.Int32, ColumnProperty.PrimaryKeyWithIdentity),
                new Column("Url", DbType.String, ColumnProperty.NotNull),
                new Column("Name", DbType.String, ColumnProperty.NotNull),
                new Column("PollInterval", DbType.Int32, ColumnProperty.NotNull),
                new Column("LastUpdateTime", DbType.DateTime, ColumnProperty.Null)
            );
        }
    }
}
