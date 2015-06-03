// LICENSE: MIT
// Author: github.com/bryceg
// Modifications by github.com/vktr

using System.Collections.Generic;
using System.Threading.Tasks;
using Autofac;
using Hadouken.Core.Security;
using Microsoft.Owin;

namespace Hadouken.Core.Http.WebSockets {
    public class WebSocketConnectionMiddleware<T> : OwinMiddleware where T : WebSocketConnection {
        private readonly ILifetimeScope _lifetimeScope;
        private readonly IUserManager _userManager;

        public WebSocketConnectionMiddleware(OwinMiddleware next,
            ILifetimeScope lifetimeScope,
            IUserManager userManager)
            : base(next) {
            this._lifetimeScope = lifetimeScope;
            this._userManager = userManager;
        }

        public override Task Invoke(IOwinContext context) {
            var token = context.Request.Query.Get("token");
            var user = this._userManager.GetUserByToken(token);

            if (user == null) {
                context.Response.StatusCode = 401;
            }
            else {
                var socketHandler = this._lifetimeScope.Resolve<T>();
                socketHandler.AcceptSocket(context, new Dictionary<string, string>());
            }

            return Task.FromResult<object>(null);
        }
    }
}