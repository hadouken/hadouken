using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using Dapper;

namespace Hadouken.Common.Data {
    public class DbConnection : IDbConnection {
        private readonly Lazy<SQLiteConnection> _connection;
        private readonly string _connectionString;

        public DbConnection(string connectionString) {
            if (connectionString == null) {
                throw new ArgumentNullException("connectionString");
            }
            this._connectionString = connectionString;
            this._connection = new Lazy<SQLiteConnection>(this.CreateConnection);
        }

        private SQLiteConnection Connection {
            get { return this._connection.Value; }
        }

        public IDbTransaction BeginTransaction() {
            return this.Connection.BeginTransaction();
        }

        public int Execute(string commandText, object parameter = null) {
            return this.Connection.Execute(commandText, parameter);
        }

        public IEnumerable<T> Query<T>(string commandText, object parameter = null) {
            return this.Connection.Query<T>(commandText, parameter);
        }

        private SQLiteConnection CreateConnection() {
            var connection = new SQLiteConnection(this._connectionString);
            connection.Open();
            connection.Execute("PRAGMA foreign_keys = ON;");

            return connection;
        }
    }
}