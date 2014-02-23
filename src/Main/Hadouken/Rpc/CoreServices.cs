using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Hadouken.Configuration;
using Hadouken.Framework.Rpc;
using Hadouken.Framework.Security;

namespace Hadouken.Rpc
{
    public class CoreServices : IJsonRpcService
    {
        private readonly IHashProvider _hashProvider;
        private readonly IConfiguration _configuration;
        private readonly IJsonRpcClient _rpcClient;

        public CoreServices(IConfiguration configuration, IJsonRpcClient rpcClient)
        {
            _hashProvider = HashProvider.GetDefault();
            _configuration = configuration;
            _rpcClient = rpcClient;
        }

        [JsonRpcMethod("core.multiCall")]
        public object MultiCall(Dictionary<string, object> call)
        {
            var result = new Dictionary<string, object>();

            foreach (var key in call.Keys)
            {
                var callResult = _rpcClient.Call<object>(key, call[key]);
                result.Add(key, callResult);
            }

            return result;
        }

        [JsonRpcMethod("core.setAuth")]
        public bool SetAuth(string userName, string newPassword, string oldPassword)
        {
            if (string.IsNullOrEmpty(userName))
                return false;

            if (string.IsNullOrEmpty(newPassword))
                return false;

            // Only compare if we have an old password
            if (!string.IsNullOrEmpty(_configuration.Http.Authentication.Password))
            {
                var oldPasswordHash = _hashProvider.ComputeHash(oldPassword);

                // If the given old username/password is incrrect, return.
                if (!string.Equals(_configuration.Http.Authentication.Password, oldPasswordHash, StringComparison.InvariantCultureIgnoreCase)
                    || !string.Equals(_configuration.Http.Authentication.UserName, userName, StringComparison.InvariantCultureIgnoreCase))
                {
                    return false;
                }
            }

            _configuration.Http.Authentication.UserName = userName;
            _configuration.Http.Authentication.Password = _hashProvider.ComputeHash(newPassword);
            _configuration.Save();

            _rpcClient.SendEvent("auth.changed", new {UserName = userName, HashedPassword = _configuration.Http.Authentication.Password});

            return true;
        }

        [JsonRpcMethod("core.getAuthInfo")]
        public object GetAuthInfo()
        {
            return new
            {
                _configuration.Http.Authentication.UserName,
                HasPassword = !String.IsNullOrEmpty(_configuration.Http.Authentication.Password)
            };
        }
    }
}
