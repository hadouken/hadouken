using System.Collections.Generic;

namespace Hadouken.Common.Data
{
    public interface IDbConnection
    {
        int Execute(string commandText, object parameter = null);

        IEnumerable<T> Query<T>(string commandText, object parameter = null);
    }
}
