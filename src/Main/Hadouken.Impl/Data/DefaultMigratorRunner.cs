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
using Migrator.Framework;
using NLog;

namespace Hadouken.Impl.Data
{
    [Component]
    public class DefaultMigratorRunner : IMigrationRunner
    {
        public void Up(Assembly target)
        {
            string dataPath = HdknConfig.GetPath("Paths.Data");
            string connectionString = HdknConfig.ConnectionString.Replace("$Paths.Data$", dataPath);

            var m = new Migrator.Migrator(new SQLiteTransformationProvider(new SQLiteDialect(), connectionString), target, false, new MigrationLogger());
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

    public class MigrationLogger : ILogger
    {
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        public void ApplyingDBChange(string sql)
        {
        }

        public void Exception(string message, Exception ex)
        {
            _logger.ErrorException(message, ex);
        }

        public void Exception(long version, string migrationName, Exception ex)
        {
            _logger.ErrorException(String.Format("Error when running migration {0}[{1}]", migrationName, version), ex);
        }

        public void Finished(List<long> currentVersion, long finalVersion)
        {
        }

        public void Log(string format, params object[] args)
        {
            _logger.Info(format, args);
        }

        public void MigrateDown(long version, string migrationName)
        {
        }

        public void MigrateUp(long version, string migrationName)
        {
        }

        public void RollingBack(long originalVersion)
        {
        }

        public void Skipping(long version)
        {
        }

        public void Started(List<long> currentVersion, long finalVersion)
        {
        }

        public void Trace(string format, params object[] args)
        {
            _logger.Trace(format, args);
        }

        public void Warn(string format, params object[] args)
        {
            _logger.Warn(format, args);
        }
    }

}
