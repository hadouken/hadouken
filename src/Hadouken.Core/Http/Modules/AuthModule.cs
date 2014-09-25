using Hadouken.Common.Logging;
using Hadouken.Core.Http.Modules.Models;
using Hadouken.Core.Http.Security;
using Hadouken.Core.Security;
using Nancy;
using Nancy.ModelBinding;

namespace Hadouken.Core.Http.Modules
{
    public sealed class AuthModule : NancyModule
    {
        public AuthModule(ILogger<AuthModule> logger,
            IUserManager userManager,
            ITokenizer tokenizer)
            : base("auth")
        {
            Get["/setup"] = _ => !userManager.HasUsers();

            Post["/login"] = _ =>
            {
                var userData = this.Bind<UserDto>();

                // First login creates user
                if (!userManager.HasUsers())
                {
                    logger.Info("Creating user account {UserName}.", userData.UserName);
                    userManager.CreateUser(userData.UserName, userData.Password);
                }

                var user = userManager.GetUser(userData.UserName, userData.Password);

                if (user == null)
                {
                    logger.Warn("Invalid username/password: {UserName}.", userData.UserName);
                    return HttpStatusCode.Unauthorized;
                }

                var identity = new UserIdentity(user.UserName, user.Claims);
                var token = tokenizer.Tokenize(identity, Context);

                return new
                {
                    Token = token,
                    identity.UserName
                };
            };
        }
    }
}
