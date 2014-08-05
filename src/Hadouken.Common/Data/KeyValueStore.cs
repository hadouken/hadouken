using System;
using System.Linq;
using Hadouken.Common.Text;

namespace Hadouken.Common.Data
{
    public class KeyValueStore : IKeyValueStore
    {
        private readonly IDbConnection _connection;
        private readonly IJsonSerializer _jsonSerializer;

        public KeyValueStore(IDbConnection connection, IJsonSerializer jsonSerializer)
        {
            if (connection == null) throw new ArgumentNullException("connection");
            if (jsonSerializer == null) throw new ArgumentNullException("jsonSerializer");
            _connection = connection;
            _jsonSerializer = jsonSerializer;
        }

        public T Get<T>(string key, T defaultValue = default(T))
        {
            var query = @"select s.Value from Setting s where s.Key = @Key";
            var result = _connection.Query<string>(query, new {Key = key}).FirstOrDefault();

            if (result == null) return defaultValue;

            return _jsonSerializer.DeserializeObject<T>(result);
        }

        public void Set<T>(string key, T value)
        {
            var findQuery = @"select exists(select 1 from Setting s where s.Key = @Key limit 1);";
            var exists = _connection.Query<bool>(findQuery, new {Key = key}).First();
            var model = new {Key = key, Value = _jsonSerializer.SerializeObject(value)};

            if (exists)
            {
                var query = @"update Setting s set s.Value = @Value where s.Key = @Key";
                _connection.Execute(query, model);
            }
            else
            {
                var query = @"insert into Setting (Key, Value) values (@Key, @Value)";
                _connection.Execute(query, model);
            }
        }
    }
}