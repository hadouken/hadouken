using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hadouken.Plugins.NoSql
{
    public interface IConfigStore
    {
        object Get(string key);

        void Set(string key, object value);

        void Delete(string key);
    }
}
