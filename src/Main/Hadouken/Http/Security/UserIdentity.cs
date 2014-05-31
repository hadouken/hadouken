using System;
using System.Collections.Generic;
using System.Linq;
using Nancy.Security;

namespace Hadouken.Http.Security
{
    public class UserIdentity : IUserIdentity
    {
        public UserIdentity(string userName, IEnumerable<string> claims)
        {
            if (userName == null) throw new ArgumentNullException("userName");
            
            UserName = userName;
            Claims = claims ?? Enumerable.Empty<string>();
        }

        public string UserName { get; private set; }

        public IEnumerable<string> Claims { get; private set; }
    }
}
