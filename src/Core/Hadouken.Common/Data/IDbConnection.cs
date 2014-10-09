using System.Collections.Generic;
using System.Data;

namespace Hadouken.Common.Data
{
    public interface IDbConnection
    {
        IDbTransaction BeginTransaction();

        int Execute(string commandText, object parameter = null);

        IEnumerable<T> Query<T>(string commandText, object parameter = null);
    }
}
