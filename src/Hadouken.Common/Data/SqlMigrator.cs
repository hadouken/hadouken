using System;
using System.IO;
using System.Linq;
using System.Text;
using Hadouken.Common.Logging;

namespace Hadouken.Common.Data
{
    public sealed class SqlMigrator : IMigrator
    {
        private static readonly string ScriptExists = "SELECT EXISTS(SELECT 1 FROM `VersionInfo` WHERE Script = @Script LIMIT 1);";
        private static readonly string InsertScript = "INSERT INTO `VersionInfo` (Script) VALUES (@Script);";

        private readonly ILogger<SqlMigrator> _logger;
        private readonly IDbConnection _connection;

        public SqlMigrator(ILogger<SqlMigrator> logger, IDbConnection connection)
        {
            if (logger == null) throw new ArgumentNullException("logger");
            if (connection == null) throw new ArgumentNullException("connection");
            _logger = logger;
            _connection = connection;
        }

        public void Migrate()
        {
            CreateVersionInfoTable();

            // Find all embedded resources ending with .sql
            var resources = (from asm in AppDomain.CurrentDomain.GetAssemblies()
                where !asm.IsDynamic
                from name in asm.GetManifestResourceNames()
                where name.EndsWith(".sql")
                let lastDotIndex = name.LastIndexOf(".")
                let dotIndex = name.Substring(0, lastDotIndex).LastIndexOf(".")
                let scriptName = name.Substring(dotIndex + 1)
                orderby scriptName ascending 
                select new {Assembly = asm, Name = name, ScriptName = scriptName});

            _logger.Info("Running migrations.");

            foreach (var resource in resources)
            {
                var applied = _connection.Query<bool>(ScriptExists, new {Script = resource.ScriptName}).First();
                if(applied) continue;

                using (var stream = resource.Assembly.GetManifestResourceStream(resource.Name))
                using (var ms = new MemoryStream())
                {
                    if (stream == null) continue;

                    stream.CopyTo(ms);
                    var sql = Encoding.UTF8.GetString(ms.ToArray());

                    _connection.Execute(sql);
                    _connection.Execute(InsertScript, new {Script = resource.ScriptName});

                    _logger.Info("Applied script {ScriptName}.", resource.ScriptName);
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
