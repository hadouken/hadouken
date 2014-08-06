using System;
using System.Collections.Generic;
using System.Linq;
using Hadouken.Common.Messaging;
using Hadouken.Common.Text;

namespace Hadouken.Common.Data
{
    public class KeyValueStore : IKeyValueStore
    {
        private class Setting
        {
            public string Key { get; set; }

            public string Value { get; set; }
        }

        private readonly IDbConnection _connection;
        private readonly IJsonSerializer _jsonSerializer;
        private readonly IMessageBus _messageBus;

        public KeyValueStore(IDbConnection connection, IJsonSerializer jsonSerializer, IMessageBus messageBus)
        {
            if (connection == null) throw new ArgumentNullException("connection");
            if (jsonSerializer == null) throw new ArgumentNullException("jsonSerializer");
            if (messageBus == null) throw new ArgumentNullException("messageBus");
            _connection = connection;
            _jsonSerializer = jsonSerializer;
            _messageBus = messageBus;
        }

        public T Get<T>(string key, T defaultValue = default(T))
        {
            var query = @"select s.Value from Setting s where s.Key = @Key";
            var result = _connection.Query<string>(query, new {Key = key}).FirstOrDefault();

            if (result == null) return defaultValue;

            return _jsonSerializer.DeserializeObject<T>(result);
        }

        public IDictionary<string, object> GetMany(string section)
        {
            var query = @"select s.Key, s.Value from Setting s where s.Key like @Section";
            var result = _connection.Query<Setting>(query, new {Section = section + "%"});

            if(result == null) return new Dictionary<string, object>();

            var dict = new Dictionary<string, object>();

            foreach (var setting in result)
            {
                var decodedValue = _jsonSerializer.DeserializeObject(setting.Value, typeof (object));
                dict.Add(setting.Key, decodedValue);
            }

            return dict;
        }

        public void Set(string key, object value)
        {
            SetInternal(key, value);
            _messageBus.Publish(new KeyValueChangedMessage(new[] {key}));
        }

        public void SetMany(IDictionary<string, object> items)
        {
            if (items == null) return;

            foreach (var item in items)
            {
                SetInternal(item.Key, item.Value);
            }

            _messageBus.Publish(new KeyValueChangedMessage(items.Keys.ToArray()));
        }

        private void SetInternal(string key, object value)
        {
            var findQuery = @"select exists(select 1 from Setting s where s.Key = @Key limit 1);";
            var exists = _connection.Query<bool>(findQuery, new { Key = key }).First();
            var model = new { Key = key, Value = _jsonSerializer.SerializeObject(value) };

            if (exists)
            {
                var query = @"update Setting set Value = @Value where Key = @Key";
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