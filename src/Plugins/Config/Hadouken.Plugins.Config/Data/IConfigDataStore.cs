using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hadouken.Plugins.Config.Data
{
    public interface IConfigDataStore
    {
        object Get(string key);

        void Set(string key, object value);

        void Delete(string key);
    }
}
