using System.Collections.Generic;

namespace Hadouken.Common.Data
{
    public interface IKeyValueStore
    {
        T Get<T>(string key, T defaultValue = default(T));

        IDictionary<string, object> GetMany(string section);

        void Set(string key, object value);

        void SetMany(IDictionary<string, object> items);
    }
}
