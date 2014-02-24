using System;
using Hadouken.Configuration;
using Hadouken.Framework.Security;
using Hadouken.Http.Management.Models;
using Nancy;
using Nancy.Authentication.Forms;
using Nancy.ModelBinding;

namespace Hadouken.Http.Management.Modules
{
    public class LoginModule : NancyModule
    {
        private readonly IConfiguration _configuration;
        private readonly IHashProvider _hashProvider = HashProvider.GetDefault();

        public LoginModule(IConfiguration configuration)
        {
            _configuration = configuration;
            Get["/login"] = _ =>
            {
                var hasAccount = !string.IsNullOrEmpty(configuration.Http.Authentication.UserName);
                return View["Index", new {AccountConfigured = hasAccount}];
            };

            Post["/login"] = _ =>
            {
                var loginParams = this.Bind<LoginParameters>();
                var hashedPassword = _hashProvider.ComputeHash(loginParams.Password);

                if (string.IsNullOrEmpty(configuration.Http.Authentication.UserName))
                {
                    SetAuthentication(loginParams.UserName, loginParams.Password);
                }

                if (!string.Equals(configuration.Http.Authentication.UserName, loginParams.UserName)
                    || !string.Equals(configuration.Http.Authentication.Password, hashedPassword))
                {
                    return "Invalid username/password.";
                }

                DateTime? rememberDate = null;

                if (loginParams.RememberMe)
                {
                    rememberDate = DateTime.Now.AddDays(30);
                }

                return this.LoginAndRedirect(Guid.NewGuid(), rememberDate);
            };
        }

        private void SetAuthentication(string userName, string password)
        {
            _configuration.Http.Authentication.UserName = userName;
            _configuration.Http.Authentication.Password = _hashProvider.ComputeHash(password);
            _configuration.Save();
        }
    }
}
