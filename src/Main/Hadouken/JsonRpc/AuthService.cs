using System;
using Hadouken.Configuration;
using Hadouken.Fx.JsonRpc;
using Hadouken.Fx.Security;

namespace Hadouken.JsonRpc
{
    public class AuthService : IJsonRpcService
    {
        private static readonly IHashProvider HashProvider = Fx.Security.HashProvider.GetDefault();
        private readonly IConfiguration _configuration;

        public AuthService(IConfiguration configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException("configuration");
            }

            _configuration = configuration;
        }

        [JsonRpcMethod("core.auth.validate")]
        public bool Validate(string userName, string password)
        {
            var passwordHash = HashProvider.ComputeHash(password ?? "");

            return string.Equals(userName, _configuration.Http.Authentication.UserName)
                   && string.Equals(passwordHash, _configuration.Http.Authentication.Password);
        }
    }
}
