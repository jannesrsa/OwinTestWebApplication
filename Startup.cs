using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Owin;
using SourceCode.Owin.Security.Authentication.Training.Middleware;
using SourceCode.Owin.Security.Authentication.WsFederation;
using System.Diagnostics;

[assembly: OwinStartup(typeof(OwinTestWebApplication.Startup))]
namespace OwinTestWebApplication
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=316888
            ConfigureAuth(app);
        }

        public void ConfigureAuth(IAppBuilder app)
        {
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = "ApplicationCookie",
                LoginPath = new PathString("/Auth/Login")
            });

            app.UseFacebookAuthentication(new Microsoft.Owin.Security.Facebook.FacebookAuthenticationOptions()
            {
                AppId = "480974769162439",
                AppSecret = "7db55e83b45f0e2da9580d88d1bf9928",
                SignInAsAuthenticationType = "ApplicationCookie"
            });

            app.Use(async (ctx, next) =>
            {
                if (ctx.Authentication.User.Identity.IsAuthenticated)
                {
                    Debug.WriteLine(string.Format("User {0} Authenticated", ctx.Authentication.User.Identity.Name));
                }
                else
                {
                    Debug.WriteLine("User Not Authenticated");
                }

                await next();
            });


            app.UseDebugMiddleware(new DebugMiddlewareOptions
            {
                OnIncommingRequest = (ctx) =>
                {
                    var watch = new Stopwatch();
                    watch.Start();
                    ctx.Environment["DebugStopwatch"] = watch;
                },

                OnOutgoingRequest = (ctx) =>
                {
                    var watch = ctx.Environment["DebugStopwatch"] as Stopwatch;
                    watch.Stop();

                    Debug.WriteLine("Time taken: {0}ms", watch.ElapsedMilliseconds);
                }
            });

            app.UseDebugMiddleware(config =>
            {
                config.OnIncommingRequest = (ctx) =>
                {
                    var watch = new Stopwatch();
                    watch.Start();
                    ctx.Environment["DebugStopwatch"] = watch;
                };

                config.OnOutgoingRequest = (ctx) =>
                {
                    var watch = ctx.Environment["DebugStopwatch"] as Stopwatch;
                    watch.Stop();

                    Debug.WriteLine("Time taken for config: {0}ms", watch.ElapsedMilliseconds);
                };
            });

            //app.Use(async (ctx, next) =>
            //{
            //    Debug.WriteLine("Incoming request: " + ctx.Request.Path);
            //    await next();
            //    Debug.WriteLine("Outgoing request: " + ctx.Request.Path);
            //});

            //app.Use(async (ctx, next) =>
            //{
            //    await ctx.Response.WriteAsync("Hello World");
            //});

            //app.UseWsFederationAuthentication();

            //app.SetDefaultSignInAsAuthenticationType(CookieAuthenticationDefaults.AuthenticationType);
        }
    }
}