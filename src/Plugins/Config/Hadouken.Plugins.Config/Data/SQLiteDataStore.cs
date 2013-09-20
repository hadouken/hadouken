using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hadouken.Plugins.Config.Data
{
    public class SQLiteDataStore : IConfigDataStore
    {
        private readonly Lazy<SQLiteConnection> _connection;

        public SQLiteDataStore()
        {
            _connection = new Lazy<SQLiteConnection>(OpenConnection);
        }

        private SQLiteConnection OpenConnection()
        {
            var dbFile = "config.db";

            var connection = new SQLiteConnection(String.Format("Data Source={0}; Version=3;", dbFile));
            connection.Open();

            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = @"create table if not exists Config
(
  Id integer primary key autoincrement,
  Key text not null unique,
  Value text
);";
                cmd.ExecuteNonQuery();
            }

            return connection;
        }

        public object Get(string key)
        {
            using (var cmd = _connection.Value.CreateCommand())
            {
                cmd.CommandText = "select Value from Config where Key = :key";
                cmd.Parameters.Add(new SQLiteParameter(":key", key));

                var data = cmd.ExecuteScalar();

                if (data == null)
                    return null;

                return JsonConvert.DeserializeObject(data.ToString());
            }
        }

        public void Set(string key, object value)
        {
            using (var cmd = _connection.Value.CreateCommand())
            {
                cmd.CommandText = "select count(*) from Config where Key = :key";
                cmd.Parameters.Add(new SQLiteParameter(":key", key));

                int rows = Convert.ToInt32(cmd.ExecuteScalar());

                if (rows > 0)
                {
                    cmd.CommandText = "update Config set Value = :value where Key = :key";
                    cmd.Parameters.Add(new SQLiteParameter(":value", JsonConvert.SerializeObject(value)));
                    cmd.Parameters.Add(new SQLiteParameter(":key", key));

                    cmd.ExecuteNonQuery();
                }
                else
                {
                    cmd.CommandText = "insert into Config(Key, Value) values(:key, :value)";
                    cmd.Parameters.Add(new SQLiteParameter(":value", JsonConvert.SerializeObject(value)));
                    cmd.Parameters.Add(new SQLiteParameter(":key", key));

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void Delete(string key)
        {
            using (var cmd = _connection.Value.CreateCommand())
            {
                cmd.CommandText = "delete from Config where Key = :key";
                cmd.Parameters.Add(new SQLiteParameter(":key", key));

                cmd.ExecuteNonQuery();
            }
        }
    }
}
