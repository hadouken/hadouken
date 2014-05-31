using Hadouken.Http.Management.Models;
using Hadouken.Http.Security;
using Nancy;
using Nancy.ModelBinding;

namespace Hadouken.Http.Management.Modules
{
    public sealed class AuthModule : NancyModule
    {
        public AuthModule(ITokenizer tokenizer)
            : base("auth")
        {
            Post["/login"] = _ =>
            {
                var loginParameters = this.Bind<LoginParameters>();

                if (!loginParameters.UserName.Equals("foo")
                    || !loginParameters.Password.Equals("bar"))
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
    }
}
