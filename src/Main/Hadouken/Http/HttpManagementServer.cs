using System;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using Hadouken.Web;
using Microsoft.Owin;
using Microsoft.Owin.FileSystems;
using Microsoft.Owin.Hosting;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.StaticFiles;
using Owin;

namespace Hadouken.Http
{
    public class HttpManagementServer : IHttpManagementServer
    {
        private readonly Microsoft.Owin.Host.HttpListener.OwinHttpListener listener__;
        private readonly Assembly _webAssembly;

        private IDisposable _httpServer;

        public HttpManagementServer()
        {
            _webAssembly = Assembly.LoadFrom("Hadouken.Web.dll");
        }

        public void Start()
        {
            _httpServer = WebApp.Start("http://localhost:7891/", BuildApplication);
        }

        public void Stop()
        {
            _httpServer.Dispose();
        }

        private void BuildApplication(IAppBuilder builder)
        {
            var cookieOpts = new CookieAuthenticationOptions
            {
                AuthenticationType = CookieAuthenticationDefaults.AuthenticationType,
                CookieName = ".hdkn_auth",
                LoginPath = new PathString("/login.html"),
                LogoutPath = new PathString("/logout")
            };

            builder.UseCookieAuthentication(cookieOpts);

            builder.Use(async (context, next) =>
            {
                if (context.Request.Path.Value.Contains(cookieOpts.LoginPath.Value))
                {
                    if (context.Request.Method == "POST")
                    {
                        // Login
                        var form = await context.Request.ReadFormAsync();
                        var userName = form["userName"];
                        var password = form["password"];

                        if (userName == "asdf" && password == "asdf")
                        {
                            var identity = new ClaimsIdentity(cookieOpts.AuthenticationType);
                            identity.AddClaim(new Claim(ClaimTypes.Name, userName));

                            // Sign in
                            context.Authentication.SignIn(identity);
                            context.Response.Redirect("/index.html");
                        }
                        else
                        {
                            var page = WebPage.GetLoginPage();

                            context.Response.ContentType = "text/html";
                            await context.Response.WriteAsync(page);
                        }
                    }
                    else
                    {
                        var page = WebPage.GetLoginPage();

                        context.Response.ContentType = "text/html";
                        await context.Response.WriteAsync(page);
                    }
                }
                else if (context.Request.Path.Value.Contains(cookieOpts.LogoutPath.Value))
                {
                    context.Authentication.SignOut(cookieOpts.AuthenticationType);
                    context.Response.Redirect(cookieOpts.LoginPath.Value);
                }
                else if (context.Request.Path.Value == "/")
                {
                    context.Response.Redirect("/index.html");
                }
                else if (context.Request.User == null || !context.Request.User.Identity.IsAuthenticated)
                {
                    context.Response.Redirect(cookieOpts.LoginPath.Value);
                }
                else
                {
                    await next();
                }
            });

            // Map static files
            var fileOpts = new StaticFileOptions
            {
                FileSystem = new EmbeddedResourceFileSystem(_webAssembly, "Hadouken.Web")
            };

            builder.UseStaticFiles(fileOpts);
        }
    }
}