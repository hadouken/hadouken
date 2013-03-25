using Hadouken.Data.Models;
using System.Collections.Generic;

namespace Hadouken.Configuration
{
    public interface IKeyValueStore : IComponent
    {
        object Get(string key);
        object Get(string key, object defaultValue);
        object Get(string key, object defaultValue, StorageLocation storageLocation);

        bool TryGet(string key, out object value);
        bool TryGet(string key, StorageLocation storageLocation, out object value);

        T Get<T>(string key);
        T Get<T>(string key, T defaultValue);
        T Get<T>(string key, T defaultValue, StorageLocation storageLocation);

        bool TryGet<T>(string key, out T value);
        bool TryGet<T>(string key, StorageLocation storageLocation, out T value);

        void Set(string key, object value);
        void Set(string key, object value, Permissions permissions, Options options);
    }
}
