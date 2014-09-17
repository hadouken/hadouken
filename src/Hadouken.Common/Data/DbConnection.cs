using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using Dapper;

namespace Hadouken.Common.Data
{
    public class DbConnection : IDbConnection
    {
        private readonly string _connectionString;
        private readonly Lazy<SQLiteConnection> _connection; 

        public DbConnection(string connectionString)
        {
            if (connectionString == null) throw new ArgumentNullException("connectionString");
            _connectionString = connectionString;
            _connection = new Lazy<SQLiteConnection>(CreateConnection);
        }

        private SQLiteConnection Connection
        {
            get { return _connection.Value; }
        }

        public IDbTransaction BeginTransaction()
        {
            return Connection.BeginTransaction();
        }

        public int Execute(string commandText, object parameter = null)
        {
            return Connection.Execute(commandText, parameter);
        }

        public IEnumerable<T> Query<T>(string commandText, object parameter = null)
        {
            return Connection.Query<T>(commandText, parameter);
        }

        private SQLiteConnection CreateConnection()
        {
            var connection = new SQLiteConnection(_connectionString);
            connection.Open();
            connection.Execute("PRAGMA foreign_keys = ON;");

            return connection;
        }
    }
}
