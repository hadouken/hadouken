using System;
using Hadouken.Configuration;
using Hadouken.Fx.Security;

namespace Hadouken.Security
{
    public class AuthenticationManager : IAuthenticationManager
    {
        private static readonly IHashProvider HashProvider = Fx.Security.HashProvider.GetDefault();
        private readonly IConfiguration _configuration;

        public AuthenticationManager(IConfiguration configuration)
        {
            if (configuration == null) throw new ArgumentNullException("configuration");
            _configuration = configuration;
        }

        public bool IsValid(string userName, string password)
        {
            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
            {
                return false;
            }

            var passwordHash = HashProvider.ComputeHash(password);

            return string.Equals(userName, _configuration.Http.Authentication.UserName)
                   && string.Equals(passwordHash, _configuration.Http.Authentication.Password);
        }
    }
}