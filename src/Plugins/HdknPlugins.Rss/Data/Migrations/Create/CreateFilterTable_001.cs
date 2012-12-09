using System.Data;
using Migrator.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HdknPlugins.Rss.Data.Migrations.Create
{
    [Migration(20121208194220)]
    public class CreateFilterTable_001 : Migration
    {
        public override void Down()
        {
            Database.RemoveTable("plugin_Rss_Filters");
        }

        public override void Up()
        {
            Database.AddTable("plugin_Rss_Filters",
                new Column("Id", DbType.Int32, ColumnProperty.PrimaryKeyWithIdentity),
                new Column("Feed_Id", DbType.Int32, ColumnProperty.NotNull),
                new Column("Label", DbType.String),
                new Column("AutoStart", DbType.Boolean),
                new Column("IncludeFilter", DbType.String),
                new Column("ExcludeFilter", DbType.String)
            );
        }
    }
}
