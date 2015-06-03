using System;
using System.Collections.Generic;
using Hadouken.Core.Security;
using Nancy.Security;

namespace Hadouken.Core.Http.Security {
    public sealed class TokenUserIdentity : IUserIdentity {
        private readonly IUser _user;

        public TokenUserIdentity(IUser user) {
            if (user == null) {
                throw new ArgumentNullException("user");
            }
            this._user = user;
        }

        public string UserName {
            get { return this._user.UserName; }
        }

        public IEnumerable<string> Claims {
            get { return new List<string>(this._user.Claims); }
        }
    }
}