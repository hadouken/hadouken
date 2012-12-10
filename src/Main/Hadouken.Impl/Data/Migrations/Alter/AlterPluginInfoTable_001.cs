using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Migrator.Framework;

namespace Hadouken.Impl.Data.Migrations.Alter
{
    [Migration(20121210124412)]
    public class AlterPluginInfoTable_001 : Migration
    {
        public override void Down()
        {
            Database.RemoveColumn("PluginInfo", "Name");
            Database.RemoveColumn("PluginInfo", "Version");
        }

        public override void Up()
        {
            Database.AddColumn("PluginInfo", new Column("Name", System.Data.DbType.String));
            Database.AddColumn("PluginInfo", new Column("Version", System.Data.DbType.String, "0.0.0.0"));
        }
    }
}
