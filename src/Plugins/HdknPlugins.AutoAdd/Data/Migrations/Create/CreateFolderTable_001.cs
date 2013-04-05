using System.Data;
using Migrator.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HdknPlugins.AutoAdd.Data.Migrations.Create
{
    [Migration(20130405175535)]
    public class CreateFolderTable_001 : Migration
    {
        public override void Down()
        {
            Database.RemoveTable("Folder");
        }

        public override void Up()
        {
            Database.AddTable("Folder",
                new Column("Id", DbType.Int32, ColumnProperty.PrimaryKeyWithIdentity),
                new Column("Path", DbType.String, ColumnProperty.NotNull),
                new Column("Label", DbType.String),
                new Column("IncludeFilter", DbType.String),
                new Column("ExcludeFilter", DbType.String),
                new Column("AutoStart", DbType.Boolean)
            );
        }
    }
}
