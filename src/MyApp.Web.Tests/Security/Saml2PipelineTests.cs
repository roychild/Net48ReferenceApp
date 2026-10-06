using System.Net;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.DataProtection;
using Microsoft.Owin.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyApp.Web.Security;
using Owin;
using Sustainsys.Saml2.Owin;

namespace MyApp.Web.Tests.Security
{
    /// <summary>
    /// Runs the Sustainsys.Saml2 middleware in an in-memory OWIN host, configured the same way as Startup.
    /// </summary>
    [TestClass]
    public class Saml2PipelineTests
    {
        private static TestServer CreateServer()
        {
            return TestServer.Create(app =>
            {
                // OWIN's default (DPAPI) protector only exists on Windows; keep the test host-independent.
                app.SetDataProtectionProvider(new PassThroughDataProtectionProvider());
                app.SetDefaultSignInAsAuthenticationType(CookieAuthenticationDefaults.AuthenticationType);
                app.UseCookieAuthentication(new CookieAuthenticationOptions());
                app.UseSaml2Authentication(Saml2OptionsFactory.Create(TestSettings.Settings()));
                app.Run(context =>
                {
                    context.Response.StatusCode = 404;
                    return Task.FromResult(0);
                });
            });
        }

        [TestMethod]
        public async Task ModulePath_PublishesServiceProviderMetadata()
        {
            using (var server = CreateServer())
            {
                var response = await server.HttpClient.GetAsync("/Saml2");

                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
                var metadata = XDocument.Parse(await response.Content.ReadAsStringAsync());
                Assert.AreEqual("https://localhost:44300/Saml2", (string)metadata.Root.Attribute("entityID"));
                StringAssert.Contains(metadata.ToString(), "/Saml2/Acs");
            }
        }

        private sealed class PassThroughDataProtectionProvider : IDataProtectionProvider, IDataProtector
        {
            public IDataProtector Create(params string[] purposes) => this;

            public byte[] Protect(byte[] userData) => userData;

            public byte[] Unprotect(byte[] protectedData) => protectedData;
        }
    }
}
