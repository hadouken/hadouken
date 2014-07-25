using System.Collections.Generic;
using Hadouken.Core.Http.Config;

namespace Hadouken.Core.Http.Security
{
    public interface IUserManager
    {
        IEnumerable<User> GetUsers();

        User GetUser(string username, string password);

        void CreateUser(string username, string password);
    }
}
