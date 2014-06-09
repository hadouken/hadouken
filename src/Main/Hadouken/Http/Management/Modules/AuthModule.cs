using System.Runtime.Remoting.Messaging;
using Hadouken.Configuration;
using Hadouken.Fx.Security;
using Hadouken.Http.Management.Models;
using Hadouken.Http.Security;
using Hadouken.Security;
using Nancy;
using Nancy.ModelBinding;

namespace Hadouken.Http.Management.Modules
{
    public sealed class AuthModule : NancyModule
    {
        public AuthModule(IConfiguration configuration, IAuthenticationManager authenticationManager, ITokenizer tokenizer)
            : base("auth")
        {
            Get["/setup"] = _ => string.IsNullOrEmpty(configuration.UserName);

            Post["/login"] = _ =>
            {
                var loginParameters = this.Bind<LoginParameters>();

                if (string.IsNullOrEmpty(configuration.UserName))
                {
                    SetAuth(configuration, loginParameters);
                }

                if (!authenticationManager.IsValid(loginParameters.UserName, loginParameters.Password))
                {
                    return HttpStatusCode.Unauthorized;
                }

                var identity = new UserIdentity(loginParameters.UserName, null);
                var token = tokenizer.Tokenize(identity, Context);

                return new
                {
                    Token = token
                };
            };
        }

        private static void SetAuth(IConfiguration configuration, LoginParameters loginParameters)
        {
            var hashProvider = HashProvider.GetDefault();
            configuration.UserName = loginParameters.UserName;
            configuration.Password = hashProvider.ComputeHash(loginParameters.Password);
            configuration.Save();
        }
    }
}
