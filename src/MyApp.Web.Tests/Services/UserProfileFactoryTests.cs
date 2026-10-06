using System.Linq;
using System.Security.Claims;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyApp.Web.Services;

namespace MyApp.Web.Tests.Services
{
    [TestClass]
    public class UserProfileFactoryTests
    {
        [TestMethod]
        public void Create_ReturnsNullForAnonymousUser()
        {
            Assert.IsNull(UserProfileFactory.Create(new ClaimsPrincipal(new ClaimsIdentity())));
            Assert.IsNull(UserProfileFactory.Create(null));
        }

        [TestMethod]
        public void Create_MapsNameIdentifierIssuerAndClaims()
        {
            var principal = TestPrincipals.Saml("jdoe@example.com", "https://idp.example.com", new Claim(ClaimTypes.Email, "jdoe@example.com"));

            var profile = UserProfileFactory.Create(principal);

            Assert.AreEqual("jdoe@example.com", profile.NameIdentifier);
            Assert.AreEqual("https://idp.example.com", profile.Issuer);
            Assert.IsTrue(profile.Claims.Any(c => c.Type == ClaimTypes.Email && c.Value == "jdoe@example.com"));
        }

        [TestMethod]
        public void Create_FallsBackToNameIdentifierWhenNoNameClaim()
        {
            var profile = UserProfileFactory.Create(TestPrincipals.Saml("user-123", "https://idp.example.com"));

            Assert.AreEqual("user-123", profile.Name);
        }

        [TestMethod]
        public void Create_PrefersNameClaim()
        {
            var principal = TestPrincipals.Saml("user-123", "https://idp.example.com", new Claim(ClaimTypes.Name, "Jane Doe"));

            Assert.AreEqual("Jane Doe", UserProfileFactory.Create(principal).Name);
        }
    }
}
