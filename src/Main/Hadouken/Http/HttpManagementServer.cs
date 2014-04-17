using System;
using System.Reflection;
using System.Security.Claims;
using Hadouken.Web;
using Microsoft.Owin;
using Microsoft.Owin.FileSystems;
using Microsoft.Owin.Hosting;
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
            var opts = new StartOptions();
            opts.Urls.Add("http://localhost:7891/");
            opts.Urls.Add("http://192.168.0.21:7891/");

            _httpServer = WebApp.Start(opts, BuildApplication);
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

            builder.MapWhen(
                ctx => !(ctx.Request.RemoteIpAddress == "127.0.0.1" || ctx.Request.RemoteIpAddress == "::1"),
                appBuilder =>
                {
                    appBuilder.UseCookieAuthentication(cookieOpts);

                    MapShared(appBuilder);

                    appBuilder.Use(async (context, next) =>
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
                        else if (context.Request.User == null || !context.Request.User.Identity.IsAuthenticated)
                        {
                            context.Response.Redirect(cookieOpts.LoginPath.Value);
                        }
                        else
                        {
                            await next();
                        }
                    });
                });

            // SignalR
            builder.MapWhen(
                ctx => (ctx.Request.RemoteIpAddress == "127.0.0.1" || ctx.Request.RemoteIpAddress == "::1"),
                MapShared);
        }

        private void MapShared(IAppBuilder builder)
        {
            builder.Use(async (ctx, next) =>
            {
                if (ctx.Request.Path.Value == "/")
                {
                    ctx.Response.Redirect("/index.html");
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

            // SignalR
            builder.MapSignalR();
        }
    }
}