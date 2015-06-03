using System;
using Hadouken.Common.JsonRpc;
using Hadouken.Core.Security;

namespace Hadouken.Core.Services {
    public sealed class UserServices : IJsonRpcService {
        private readonly IUserManager _userManager;

        public UserServices(IUserManager userManager) {
            if (userManager == null) {
                throw new ArgumentNullException("userManager");
            }
            this._userManager = userManager;
        }

        [JsonRpcMethod("users.changePassword")]
        public void ChangePassword(string userName, string oldPassword, string newPassword) {
            var user = this._userManager.GetUser(userName, oldPassword);

            if (user == null) {
                throw new JsonRpcException(1000, "Invalid username/password.");
            }

            this._userManager.ChangePassword(userName, newPassword);
        }
    }
}