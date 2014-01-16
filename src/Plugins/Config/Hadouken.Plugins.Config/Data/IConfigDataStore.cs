using System.Collections.Generic;

namespace Hadouken.Plugins.Config.Data
{
    public interface IConfigDataStore
    {
        object Get(string key);

        IDictionary<string, object> GetStartingWith(string section); 

        void Set(string key, object value);

        void Delete(string key);

        void Save();
    }
}
