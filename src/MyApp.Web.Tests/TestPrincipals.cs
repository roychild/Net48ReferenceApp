using System.Security.Claims;

namespace MyApp.Web.Tests
{
    internal static class TestPrincipals
    {
        public static ClaimsPrincipal Anonymous => new ClaimsPrincipal(new ClaimsIdentity());

        /// <summary>A principal shaped like the one Sustainsys.Saml2 produces from a SAML response.</summary>
        public static ClaimsPrincipal Saml(string nameId, string issuer, params Claim[] extraClaims)
        {
            var identity = new ClaimsIdentity("Federation");
            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, nameId, ClaimValueTypes.String, issuer));
            identity.AddClaims(extraClaims);
            return new ClaimsPrincipal(identity);
        }
    }
}
