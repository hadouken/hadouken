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

            var query = @"insert or replace into Setting(Key, Value) values(@Key, @Value);";
            var model = (from item in items
                         select new
                         {
                             item.Key,
                             Value = _jsonSerializer.SerializeObject(item.Value)
                         }).ToArray();

            _connection.Execute(query, model);
            _messageBus.Publish(new KeyValueChangedMessage(items.Keys.ToArray()));
        }

        private void SetInternal(string key, object value)
        {
            var query = @"insert or replace into Setting(Key, Value) values(@Key, @Value);";
            var model = new { Key = key, Value = _jsonSerializer.SerializeObject(value) };

            _connection.Execute(query, model);
        }
    }
}