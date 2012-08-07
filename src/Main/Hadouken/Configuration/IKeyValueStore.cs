using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace Hadouken.Configuration
{
    public interface IKeyValueStore : IComponent
    {
        object Get(string key);
        object Get(string key, object defaultValue);

        IDictionary<string, object> Get(Func<string, bool> filter);

        T Get<T>(string key);
        T Get<T>(string key, T defaultValue);

        void Set(string key, object value);
    }
}
