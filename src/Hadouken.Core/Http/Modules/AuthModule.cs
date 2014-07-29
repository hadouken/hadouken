using System.Linq;
using Hadouken.Common.Logging;
using Hadouken.Core.Http.Modules.Models;
using Hadouken.Core.Http.Security;
using Nancy;
using Nancy.ModelBinding;

namespace Hadouken.Core.Http.Modules
{
    public sealed class AuthModule : NancyModule
    {
        public AuthModule(ILogger logger,
            IUserManager userManager,
            ITokenizer tokenizer)
            : base("auth")
        {
            Get["/setup"] = _ => !userManager.GetUsers().Any();

            Post["/login"] = _ =>
            {
                var userData = this.Bind<UserDto>();

                // First login creates user
                if (!userManager.GetUsers().Any())
                {
                    logger.Info("Creating user account " + userData.UserName);
                    userManager.CreateUser(userData.UserName, userData.Password);
                }

                var user = userManager.GetUser(userData.UserName, userData.Password);

                if (user == null)
                {
                    logger.Warn("Invalid username/password, account " + userData.UserName);
                    return HttpStatusCode.Unauthorized;
                }

                var identity = new UserIdentity(user.UserName, user.Claims);
                var token = tokenizer.Tokenize(identity, Context);

                return new
                {
                    Token = token
                };
            };
        }
    }
}
