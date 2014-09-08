using System;
using System.Collections.Generic;
using System.Data.SQLite;
using Dapper;

namespace Hadouken.Common.Data
{
    public class DbConnection : IDbConnection
    {
        private readonly string _connectionString;

        public DbConnection(string connectionString)
        {
            if (connectionString == null) throw new ArgumentNullException("connectionString");
            _connectionString = connectionString;
        }

        public int Execute(string commandText, object parameter = null)
        {
            using (var connection = CreateConnection())
            {
                return connection.Execute(commandText, parameter);
            }
        }

        public IEnumerable<T> Query<T>(string commandText, object parameter = null)
        {
            using (var connection = CreateConnection())
            {
                return connection.Query<T>(commandText, parameter);
            }
        }

        private SQLiteConnection CreateConnection()
        {
            var connection = new SQLiteConnection(_connectionString);
            connection.Execute("PRAGMA foreign_keys = ON;");

            return connection;
        }
    }
}
