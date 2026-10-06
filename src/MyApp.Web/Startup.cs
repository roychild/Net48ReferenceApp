using System;
using Microsoft.Owin;
using Microsoft.Owin.Extensions;
using Microsoft.Owin.Host.SystemWeb;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using MyApp.Web.Security;
using Owin;
using Sustainsys.Saml2.Owin;

[assembly: OwinStartup(typeof(MyApp.Web.Startup))]

namespace MyApp.Web
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.SetDefaultSignInAsAuthenticationType(CookieAuthenticationDefaults.AuthenticationType);

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = CookieAuthenticationDefaults.AuthenticationType,
                CookieName = "MyApp.Auth",
                LoginPath = new PathString("/account/login"),
                CookieHttpOnly = true,
                CookieSecure = CookieSecureOption.Always,
                ExpireTimeSpan = TimeSpan.FromHours(8),
                SlidingExpiration = true,
                // Avoids the System.Web / OWIN cookie collision ("cookie monster") bug.
                CookieManager = new SystemWebCookieManager(),
                Provider = new CookieAuthenticationProvider
                {
                    OnApplyRedirect = context =>
                    {
                        // API callers get a plain 401 rather than a redirect to the login page.
                        if (!IsApiRequest(context.Request))
                        {
                            context.Response.Redirect(context.RedirectUri);
                        }
                    },
                },
            });

            app.UseSaml2Authentication(Saml2OptionsFactory.Create(Saml2Settings.FromAppSettings()));

            app.UseStageMarker(PipelineStage.Authenticate);
        }

        internal static bool IsApiRequest(IOwinRequest request)
        {
            return request.Path.StartsWithSegments(new PathString("/api"));
        }
    }
}
