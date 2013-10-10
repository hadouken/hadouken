using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using Hadouken.Configuration;
using Hadouken.Framework.Rpc;
using Newtonsoft.Json.Linq;

namespace Hadouken.Rpc
{
    public class CoreServices : IJsonRpcService
    {
        private readonly IConfiguration _configuration;
        private readonly IJsonRpcClient _rpcClient;

        public CoreServices(IConfiguration configuration, IJsonRpcClient rpcClient)
        {
            _configuration = configuration;
            _rpcClient = rpcClient;
        }

        [JsonRpcMethod("core.multiCall")]
        public object MultiCall(Dictionary<string, object> call)
        {
            var result = new Dictionary<string, object>();

            foreach (var key in call.Keys)
            {
                var callResult = _rpcClient.CallAsync<object>(key, call[key]).Result;
                result.Add(key, callResult);
            }

            return result;
        }

        [JsonRpcMethod("core.setAuth")]
        public bool SetAuth(string userName, string newPassword, string oldPassword)
        {
            if (String.IsNullOrEmpty(userName))
                return false;

            if (String.IsNullOrEmpty(newPassword))
                return false;

            // Only compare if we have an old password
            if(!String.IsNullOrEmpty(_configuration.Http.Authentication.Password)) {
                var oldPasswordHash = ComputeHash(oldPassword);

                // If the given old password is incrrect, return.
                if (!String.Equals(_configuration.Http.Authentication.Password, oldPasswordHash, StringComparison.InvariantCultureIgnoreCase))
                {
                    return false;
                }
            }

            _configuration.Http.Authentication.UserName = userName;
            _configuration.Http.Authentication.Password = ComputeHash(newPassword);
            _configuration.Save();

            _rpcClient.SendEventAsync("auth.changed", new {UserName = userName, HashedPassword = _configuration.Http.Authentication.Password});

            return true;
        }

        [JsonRpcMethod("core.getAuthInfo")]
        public object GetAuthInfo()
        {
            return new
            {
                Username = _configuration.Http.Authentication.UserName,
                HasPassword = !String.IsNullOrEmpty(_configuration.Http.Authentication.Password)
            };
        }

        private string ComputeHash(string input)
        {
            var bytes = Encoding.UTF8.GetBytes(input);
            var sha256 = new SHA256Managed();
            var hash = sha256.ComputeHash(bytes);
            var sb = new StringBuilder();

            foreach (var b in hash)
            {
                sb.AppendFormat("{0:x2}", b);
            }

            return sb.ToString();
        }
    }
}
