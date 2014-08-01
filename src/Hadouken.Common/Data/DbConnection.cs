using System;
using System.Collections.Generic;
using System.Data.SQLite;
using Dapper;

namespace Hadouken.Common.Data
{
    public class DbConnection : IDbConnection
    {
        private readonly string _connectionString;

        public DbConnection(IEnvironment environment)
        {
            if (environment == null) throw new ArgumentNullException("environment");

            var dataPath = environment.GetApplicationDataPath();
            var connectionString = environment.GetAppSetting("DbConnection").Replace("${Data}", dataPath.FullPath);

            _connectionString = connectionString;
        }

        public int Execute(string commandText, object parameter = null)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                return connection.Execute(commandText, parameter);
            }
        }

        public IEnumerable<T> Query<T>(string commandText, object parameter = null)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                return connection.Query<T>(commandText, parameter);
            }
        }
    }
}
