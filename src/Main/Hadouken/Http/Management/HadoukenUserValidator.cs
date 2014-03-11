using System;
using Hadouken.Configuration;
using Hadouken.Fx.Security;
using Nancy.Authentication.Basic;
using Nancy.Security;

namespace Hadouken.Http.Management
{
    public class HadoukenUserValidator : IUserValidator
    {
        private static readonly IHashProvider HashProvider = Fx.Security.HashProvider.GetDefault();
        private readonly IConfiguration _configuration;

        public HadoukenUserValidator(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IUserIdentity Validate(string username, string password)
        {
            var hashedPassword = HashProvider.ComputeHash(password);

            if (username ==_configuration.Http.Authentication.UserName
                && hashedPassword == _configuration.Http.Authentication.Password)
            {
                return new HadoukenIdentity {UserName = username};
            }

            return null;
        }
    }
}