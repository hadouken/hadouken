using System;
using Hadouken.Core.Security;

namespace Hadouken.Core.Http.Security
{
    public class TokenAuthenticationConfiguration
    {
        public TokenAuthenticationConfiguration(IUserManager userManager)
        {
            if (userManager == null)
            {
                throw new ArgumentNullException("userManager");
            }

            UserManager = userManager;
        }

        public IUserManager UserManager { get; private set; }
    }
}
