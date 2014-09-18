using System;
using System.IO;
using System.Linq;
using System.Text;
using Hadouken.Common.Logging;
using Hadouken.Common.Reflection;

namespace Hadouken.Common.Data
{
    public sealed class SqlMigrator : IMigrator
    {
        private static readonly string ScriptExists = "SELECT EXISTS(SELECT 1 FROM `VersionInfo` WHERE Script = @Script LIMIT 1);";
        private static readonly string InsertScript = "INSERT INTO `VersionInfo` (Script) VALUES (@Script);";

        private readonly ILogger<SqlMigrator> _logger;
        private readonly IDbConnection _connection;
        private readonly IEmbeddedResourceFinder _embeddedResourceFinder;

        public SqlMigrator(ILogger<SqlMigrator> logger,
            IDbConnection connection,
            IEmbeddedResourceFinder embeddedResourceFinder)
        {
            if (logger == null) throw new ArgumentNullException("logger");
            if (connection == null) throw new ArgumentNullException("connection");
            if (embeddedResourceFinder == null) throw new ArgumentNullException("embeddedResourceFinder");

            _logger = logger;
            _connection = connection;
            _embeddedResourceFinder = embeddedResourceFinder;
        }

        public void Migrate()
        {
            CreateVersionInfoTable();

            _logger.Info("Running migrations.");

            var resources = _embeddedResourceFinder.GetAll()
                .Where(r => r.Name.EndsWith(".sql"));

            foreach (var resource in resources)
            {
                var applied = _connection.Query<bool>(ScriptExists, new {Script = resource.Name}).First();
                if(applied) continue;

                using (var stream = resource.OpenRead())
                using (var ms = new MemoryStream())
                {
                    if (stream == null) continue;

                    stream.CopyTo(ms);
                    var sql = Encoding.UTF8.GetString(ms.ToArray());

                    using (var transaction = _connection.BeginTransaction())
                    {
                        _connection.Execute(sql);
                        _connection.Execute(InsertScript, new {Script = resource.Name});

                        transaction.Commit();
                    }

                    _logger.Info("Applied script {ScriptName}.", resource.Name);
                }
            }
        }

        private void CreateVersionInfoTable()
        {
            var createTable = @"CREATE TABLE IF NOT EXISTS `VersionInfo` (
    Script TEXT NOT NULL,
    CONSTRAINT 'UQ_Script' UNIQUE ('Script')
);";

            _connection.Execute(createTable);
        }
    }
}
