using Hadouken.Data.Models;
using System.Collections.Generic;

namespace Hadouken.Configuration
{
    public interface IKeyValueStore
    {
        object Get(string key);
        object Get(string key, object defaultValue);

        bool TryGet(string key, out object value);

        T Get<T>(string key);
        T Get<T>(string key, T defaultValue);

        bool TryGet<T>(string key, out T value);

        void Set(string key, object value);
        void Set(string key, object value, Permissions permissions, Options options);
    }
}
