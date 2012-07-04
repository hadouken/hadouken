using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;

using Hadouken.Data;
using Migrator.Providers.SQLite;

namespace Hadouken.Impl.Data
{
    public class DefaultMigratorRunner : IMigratorRunner
    {
        public void Run(Assembly target)
        {
            var m = new Migrator.Migrator(new SQLiteTransformationProvider(new SQLiteDialect(), ConfigurationManager.ConnectionStrings["hdkn"].ConnectionString), target, false);
            m.MigrateToLastVersion();
        }
    }
}
