using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Migrator.Framework;

namespace Hadouken.Impl.Data.Migrations.Insert
{
    [Migration(201210060022)]
    public class InsertSettingData_001 : Migration
    {
        public override void Down()
        {
            Database.Delete("Setting", "Key", "webui.cookie");

            Database.Delete("Setting", "Key", "gui.tall_category_list");

            Database.Delete("Setting", "Key", "paths.shouldMoveCompleted");
            Database.Delete("Setting", "Key", "paths.completedPath");
            Database.Delete("Setting", "Key", "paths.appendLabelOnMoveCompleted");
        }

        public override void Up()
        {
            Database.Insert("Setting", new[] { "Key", "Value", "Type" }, new[] { "webui.cookie", "\"{}\"", "System.String" });

            // default gui settings
            Database.Insert("Setting", new[] { "Key", "Value", "Type" }, new[] { "gui.tall_category_list", "true", "System.Boolean" });

            // Move on completed?
            Database.Insert("Setting", new[] { "Key", "Value", "Type" }, new[] { "paths.shouldMoveCompleted", "false", "System.Boolean" });
            Database.Insert("Setting", new[] { "Key", "Value", "Type" }, new[] { "paths.completedPath", "\"\"", "System.String" });
            Database.Insert("Setting", new[] { "Key", "Value", "Type" }, new[] { "paths.appendLabelOnMoveCompleted", "false", "System.Boolean" });
        }
    }
}
