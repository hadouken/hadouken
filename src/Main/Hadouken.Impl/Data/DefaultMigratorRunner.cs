using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;

using Hadouken.Data;
using Migrator.Providers.SQLite;
using System.IO;
using Hadouken.Configuration;

namespace Hadouken.Impl.Data
{
    public class DefaultMigratorRunner : IMigrationRunner
    {
        public void Up(Assembly target)
        {
            string dataPath = HdknConfig.GetPath("Paths.Data");
            string connectionString = HdknConfig.ConnectionString.Replace("$Paths.Data$", dataPath);

            var m = new Migrator.Migrator(new SQLiteTransformationProvider(new SQLiteDialect(), connectionString), target, false);
            m.MigrateToLastVersion();
        }

        public void Down(Assembly target)
        {
            string dataPath = HdknConfig.GetPath("Paths.Data");
            string connectionString = HdknConfig.ConnectionString.Replace("$Paths.Data$", dataPath);

            var m = new Migrator.Migrator(new SQLiteTransformationProvider(new SQLiteDialect(), connectionString), target, false);
            m.MigrateTo(-1);
        }
    }
}
