using System.Collections.Specialized;
using MyApp.Web.Security;

namespace MyApp.Web.Tests.Security
{
    internal static class TestSettings
    {
        public static NameValueCollection Values()
        {
            return new NameValueCollection
            {
                { "Saml2:SpEntityId", "https://localhost:44300/Saml2" },
                { "Saml2:ReturnUrl", "https://localhost:44300/" },
                { "Saml2:IdpEntityId", "https://idp.example.com/metadata" },
                { "Saml2:LoadIdpMetadata", "false" },
            };
        }

        public static Saml2Settings Settings()
        {
            return Saml2Settings.FromNameValueCollection(Values());
        }
    }
}
