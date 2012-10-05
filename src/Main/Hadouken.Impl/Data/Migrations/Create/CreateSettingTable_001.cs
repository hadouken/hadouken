using Migrator.Framework;

namespace Hadouken.Impl.Data.Migrations.Create
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
        }

        public override void Down()
        {
            Database.RemoveTable("Setting");
        }
    }
}
