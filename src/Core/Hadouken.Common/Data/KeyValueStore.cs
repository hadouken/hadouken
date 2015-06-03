using System;
using System.Collections.Generic;
using System.Linq;
using Hadouken.Common.Messaging;
using Hadouken.Common.Text;

namespace Hadouken.Common.Data {
    public class KeyValueStore : IKeyValueStore {
        private readonly IDbConnection _connection;
        private readonly IJsonSerializer _jsonSerializer;
        private readonly IMessageBus _messageBus;

        public KeyValueStore(IDbConnection connection, IJsonSerializer jsonSerializer, IMessageBus messageBus) {
            if (connection == null) {
                throw new ArgumentNullException("connection");
            }
            if (jsonSerializer == null) {
                throw new ArgumentNullException("jsonSerializer");
            }
            if (messageBus == null) {
                throw new ArgumentNullException("messageBus");
            }
            this._connection = connection;
            this._jsonSerializer = jsonSerializer;
            this._messageBus = messageBus;
        }

        public T Get<T>(string key, T defaultValue = default(T)) {
            const string query = @"select s.Value from Setting s where s.Key = @Key";
            var result = this._connection.Query<string>(query, new {Key = key}).FirstOrDefault();

            return result == null ? defaultValue : this._jsonSerializer.DeserializeObject<T>(result);
        }

        public IDictionary<string, object> GetMany(string section) {
            const string query = @"select s.Key, s.Value from Setting s where s.Key like @Section";
            var result = this._connection.Query<Setting>(query, new {Section = section + "%"});

            if (result == null) {
                return new Dictionary<string, object>();
            }

            var dict = new Dictionary<string, object>();

            foreach (var setting in result) {
                var decodedValue = this._jsonSerializer.DeserializeObject(setting.Value, typeof (object));
                dict.Add(setting.Key, decodedValue);
            }

            return dict;
        }

        public void Set(string key, object value) {
            this.SetInternal(key, value);
            this._messageBus.Publish(new KeyValueChangedMessage(new[] {key}));
        }

        public void SetMany(IDictionary<string, object> items) {
            if (items == null || !items.Any()) {
                return;
            }

            const string query = @"insert or replace into Setting(Key, Value) values(@Key, @Value);";
            var model = (from item in items
                select new {
                    item.Key,
                    Value = this._jsonSerializer.SerializeObject(item.Value)
                }).ToArray();

            this._connection.Execute(query, model);
            this._messageBus.Publish(new KeyValueChangedMessage(items.Keys.ToArray()));
        }

        private void SetInternal(string key, object value) {
            const string query = @"insert or replace into Setting(Key, Value) values(@Key, @Value);";
            var model = new {Key = key, Value = this._jsonSerializer.SerializeObject(value)};

            this._connection.Execute(query, model);
        }

        private class Setting {
            public string Key { get; set; }
            public string Value { get; set; }
        }
    }
}