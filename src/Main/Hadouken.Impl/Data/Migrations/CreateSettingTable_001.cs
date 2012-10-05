using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Migrator;
using Migrator.Framework;

namespace Hadouken.Impl.Data.Migrations
{
    [Migration(201206201328)]
    public class CreateSettingTable_001 : Migration
    {
        public override void Up()
        {
            Database.AddTable("Setting",
                new Column("Id", System.Data.DbType.Int32, ColumnProperty.PrimaryKeyWithIdentity),
                new Column("Key", System.Data.DbType.String, 200),
                new Column("Value", System.Data.DbType.String, 200),
                new Column("Type", System.Data.DbType.String, 200),
                new Column("Permissions", System.Data.DbType.Int32)
            );

            Database.Insert("Setting", new [] { "Key", "Value", "Type" }, new [] { "webui.cookie", "\"{}\"", "System.String" });

            // default gui settings
            Database.Insert("Setting", new [] { "Key", "Value", "Type" }, new [] { "gui.tall_category_list", "true", "System.Boolean" });

            // Move on completed?
            Database.Insert("Setting", new [] { "Key", "Value", "Type" }, new [] { "paths.shouldMoveCompleted", "false", "System.Boolean" });
            Database.Insert("Setting", new [] { "Key", "Value", "Type" }, new [] { "paths.completedPath", "\"\"", "System.String" });
            Database.Insert("Setting", new [] { "Key", "Value", "Type" }, new [] { "paths.appendLabelOnMoveCompleted", "false", "System.Boolean" });
        }

        public override void Down()
        {
            Database.RemoveTable("Setting");
        }
    }
}
