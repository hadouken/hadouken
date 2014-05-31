using System;
using Hadouken.Fx.JsonRpc;
using Hadouken.Security;

namespace Hadouken.JsonRpc
{
    public class AuthService : IJsonRpcService
    {
        private readonly IAuthenticationManager _authenticationManager;

        public AuthService(IAuthenticationManager authenticationManager)
        {
            if (authenticationManager == null) throw new ArgumentNullException("authenticationManager");
            _authenticationManager = authenticationManager;
        }

        [JsonRpcMethod("core.auth.validate")]
        public bool Validate(string userName, string password)
        {
            return _authenticationManager.IsValid(userName, password);
        }
    }
}
