using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Migrator.Framework;

namespace Hadouken.Impl.Data.Migrations.Drop
{
    [Migration(20130425182541)]
    public class DropPluginInfoTable_001 : Migration
    {
        public override void Down()
        {
        }

        public override void Up()
        {
            Database.RemoveTable("PluginInfo");
        }
    }
}
