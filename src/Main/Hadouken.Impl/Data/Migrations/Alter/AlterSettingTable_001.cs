using System.Data;
using Migrator.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hadouken.Impl.Data.Migrations.Alter
{
    [Migration(20130204230000)]
    public class AlterSettingTable_001 : Migration
    {
        public override void Down()
        {
            Database.RemoveColumn("Setting", "Options");
        }

        public override void Up()
        {
            Database.AddColumn("Setting", new Column("Options", DbType.Int32, ColumnProperty.NotNull, 0));
        }
    }
}
