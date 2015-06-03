using System;
using System.IO;
using System.Linq;
using System.Text;
using Hadouken.Common.Logging;
using Hadouken.Common.Reflection;

namespace Hadouken.Common.Data {
    public sealed class SqlMigrator : IMigrator {
        private const string ScriptExists = "SELECT EXISTS(SELECT 1 FROM `VersionInfo` WHERE Script = @Script LIMIT 1);";

        private const string InsertScript = "INSERT INTO `VersionInfo` (Script) VALUES (@Script);";
        private readonly IDbConnection _connection;
        private readonly IEmbeddedResourceFinder _embeddedResourceFinder;
        private readonly ILogger<SqlMigrator> _logger;

        public SqlMigrator(ILogger<SqlMigrator> logger,
            IDbConnection connection,
            IEmbeddedResourceFinder embeddedResourceFinder) {
            if (logger == null) {
                throw new ArgumentNullException("logger");
            }
            if (connection == null) {
                throw new ArgumentNullException("connection");
            }
            if (embeddedResourceFinder == null) {
                throw new ArgumentNullException("embeddedResourceFinder");
            }

            this._logger = logger;
            this._connection = connection;
            this._embeddedResourceFinder = embeddedResourceFinder;
        }

        public void Migrate() {
            this.CreateVersionInfoTable();

            this._logger.Info("Running migrations.");

            var resources = this._embeddedResourceFinder.GetAll()
                .Where(r => r.Name.EndsWith(".sql"));

            foreach (var resource in resources) {
                var applied = this._connection.Query<bool>(ScriptExists, new {Script = resource.Name}).First();
                if (applied) {
                    continue;
                }

                using (var stream = resource.OpenRead()) {
                    using (var ms = new MemoryStream()) {
                        if (stream == null) {
                            continue;
                        }

                        stream.CopyTo(ms);
                        var sql = Encoding.UTF8.GetString(ms.ToArray());

                        using (var transaction = this._connection.BeginTransaction()) {
                            this._connection.Execute(sql);
                            this._connection.Execute(InsertScript, new {Script = resource.Name});

                            transaction.Commit();
                        }

                        this._logger.Info("Applied script {ScriptName}.", resource.Name);
                    }
                }
            }
        }

        private void CreateVersionInfoTable() {
            const string createTable = @"CREATE TABLE IF NOT EXISTS `VersionInfo` (
    Script TEXT NOT NULL,
    CONSTRAINT 'UQ_Script' UNIQUE ('Script')
);";

            this._connection.Execute(createTable);
        }
    }
}