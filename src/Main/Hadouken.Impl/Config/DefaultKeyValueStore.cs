using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hadouken.Common.Data;
using Hadouken.Common.Messaging;
using Hadouken.Configuration;
using Hadouken.Data;
using Hadouken.Data.Models;
using System.Web.Script.Serialization;
using System.Linq.Expressions;

using Hadouken.Security;

using Microsoft.Win32;

namespace Hadouken.Impl.Config
{
    public class DefaultKeyValueStore : IKeyValueStore
    {
        private readonly JavaScriptSerializer _serializer = new JavaScriptSerializer();
        private readonly IDataRepository _data;
        private readonly IMessageBus _bus;

        public DefaultKeyValueStore(IDataRepository data, IMessageBus bus)
        {
            _data = data;
            _bus = bus;
        }

        public object Get(string key)
        {
            return InternalGet(key, null, false);
        }

        public object Get(string key, object defaultValue)
        {
            return InternalGet(key, defaultValue, true);
        }

        private object InternalGet(string key, object defaultValue, bool userProvidedDefaultValue)
        {
            object v;

            if (TryGet(key, out v))
            {
                return v;
            }

            if (userProvidedDefaultValue)
                return defaultValue;

            throw new Exception("Key does not exist: " + key);
        }

        public bool TryGet(string key, out object value)
        {
            object internalValue = GetFromDatabase(key);;

            if (internalValue != null)
            {
                value = internalValue;
                return true;
            }

            value = null;
            return false;
        }

        private object GetFromDatabase(string key)
        {
            var setting = GetSettingFromDatabase(key);

            if (setting == null)
                return null;

            var type = Type.GetType(setting.Type);

            if (type == null)
                throw new Exception("Cannot deserialize setting with no known type");

            return _serializer.Deserialize(setting.Value, type);
        }

        private T GetFromDatabase<T>(string key)
        {
            var setting = GetSettingFromDatabase(key);

            if (setting == null)
                return default(T);

            return _serializer.Deserialize<T>(setting.Value);
        }

        private Setting GetSettingFromDatabase(string key)
        {
            var setting = _data.Single<Setting>(s => s.Key == key);

            if (setting == null)
                return null;

            if (!setting.Permissions.HasFlag(Permissions.Read) && setting.Permissions.HasFlag(Permissions.Write))
                throw new UnauthorizedAccessException("Key " + key + " is write-only.");

            return setting;
        }

        public T Get<T>(string key)
        {
            return InternalGet<T>(key, default(T), false);
        }

        public T Get<T>(string key, T defaultValue)
        {
            return InternalGet<T>(key, defaultValue, true);
        }

        public T InternalGet<T>(string key, T defaultValue, bool userProvidedDefaultValue)
        {
            T v;

            if (TryGet<T>(key, out v))
            {
                return v;
            }

            if (userProvidedDefaultValue)
                return defaultValue;

            throw new Exception("Key does not exist: " + key);
        }

        public bool TryGet<T>(string key, out T value)
        {
            T internalValue = GetFromDatabase<T>(key);
            bool hasValue = !EqualityComparer<T>.Default.Equals(internalValue, default(T));

            value = internalValue;

            if (hasValue)
                return true;

            return false;
        }

        public void Set(string key, object value)
        {
            Set(key, value, Permissions.Read | Permissions.Write, Options.None);
        }

        public void Set(string key, object value, Permissions permissions, Options options)
        {
            if (key == null)
                throw new ArgumentNullException("key");

            var setting = _data.Single<Setting>(s => s.Key == key);

            if (setting == null)
                setting = new Setting
                {
                    Key = key,
                    Type = value.GetType().FullName,
                    Permissions = permissions,
                    Options = options
                };

            if (!setting.Permissions.HasFlag(Permissions.Write))
                throw new UnauthorizedAccessException("No write permissions on key " + key);

            setting.Value = _serializer.Serialize(setting.Options.HasFlag(Options.Hashed) ? Hash.Generate(value.ToString()) : value);

            _data.SaveOrUpdate(setting);

            // Send ISettingChanged message
            _bus.Publish(new KeyValueChangedMessage { Key = setting.Key, NewValue = value.ToString()});
        }
    }
}
