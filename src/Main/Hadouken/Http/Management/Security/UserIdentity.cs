using System.Collections.Generic;
using System.Linq;
using Nancy.Security;

namespace Hadouken.Http.Management.Security
{
    public class UserIdentity : IUserIdentity
    {
        public UserIdentity(string userName)
        {
            UserName = userName;
            Claims = Enumerable.Empty<string>();
        }

        public string UserName { get; private set; }

        public IEnumerable<string> Claims { get; private set; }
    }
}