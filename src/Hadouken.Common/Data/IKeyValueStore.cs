using System.Collections.Generic;

namespace Hadouken.Common.Data
{
    public interface IKeyValueStore
    {
        /// <summary>
        /// Get a specific value from the storage.
        /// </summary>
        /// <typeparam name="T">The <see cref="System.Type"/> to deserialize the value as.</typeparam>
        /// <param name="key">The key to look for.</param>
        /// <param name="defaultValue">An optional default value which will be returned if the key is not found.</param>
        /// <returns>The stored value, or the default value if the key is missing.</returns>
        T Get<T>(string key, T defaultValue = default(T));

        IDictionary<string, object> GetMany(string section);

        void Set(string key, object value);

        void SetMany(IDictionary<string, object> items);
    }
}
