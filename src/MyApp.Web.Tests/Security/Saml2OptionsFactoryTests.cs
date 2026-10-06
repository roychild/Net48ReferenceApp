using System;
using System.Linq;
using System.Security.Claims;
using Microsoft.Owin.Security;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyApp.Web.Security;

namespace MyApp.Web.Tests.Security
{
    [TestClass]
    public class Saml2OptionsFactoryTests
    {
        [TestMethod]
        public void Create_ConfiguresServiceProvider()
        {
            var options = Saml2OptionsFactory.Create(TestSettings.Settings());

            Assert.AreEqual(Saml2OptionsFactory.AuthenticationType, options.AuthenticationType);
            Assert.AreEqual(AuthenticationMode.Passive, options.AuthenticationMode);
            Assert.AreEqual("https://localhost:44300/Saml2", options.SPOptions.EntityId.Id);
            Assert.AreEqual(new Uri("https://localhost:44300/"), options.SPOptions.ReturnUrl);
            Assert.AreEqual("/Saml2", options.SPOptions.ModulePath);
        }

        [TestMethod]
        public void Create_RegistersIdentityProvider()
        {
            var options = Saml2OptionsFactory.Create(TestSettings.Settings());

            var idp = options.IdentityProviders.KnownIdentityProviders.Single();
            Assert.AreEqual("https://idp.example.com/metadata", idp.EntityId.Id);
            Assert.IsFalse(idp.AllowUnsolicitedAuthnResponse);
        }

        [TestMethod]
        public void Create_DoesNotLoadCertificateWhenNoThumbprintConfigured()
        {
            var options = Saml2OptionsFactory.Create(TestSettings.Settings(), _ => throw new AssertFailedException("Should not load a certificate."));

            Assert.AreEqual(0, options.SPOptions.ServiceCertificates.Count);
        }

        [TestMethod]
        public void Create_LoadsSigningCertificateByThumbprint()
        {
            var settings = TestSettings.Settings();
            settings.SigningCertificateThumbprint = "ABC123";
            string requested = null;

            try
            {
                Saml2OptionsFactory.Create(settings, thumbprint =>
                {
                    requested = thumbprint;
                    throw new InvalidOperationException("stop");
                });
            }
            catch (InvalidOperationException)
            {
            }

            Assert.AreEqual("ABC123", requested);
        }

        [TestMethod]
        public void EnsureNameClaim_CopiesNameIdentifierWhenNameMissing()
        {
            var identity = (ClaimsIdentity)TestPrincipals.Saml("jdoe", "https://idp.example.com").Identity;

            Saml2OptionsFactory.EnsureNameClaim(identity);

            Assert.AreEqual("jdoe", identity.Name);
            Assert.AreEqual("https://idp.example.com", identity.FindFirst(ClaimTypes.Name).Issuer);
        }

        [TestMethod]
        public void EnsureNameClaim_KeepsExistingName()
        {
            var identity = (ClaimsIdentity)TestPrincipals.Saml("jdoe", "https://idp.example.com", new Claim(ClaimTypes.Name, "Jane Doe")).Identity;

            Saml2OptionsFactory.EnsureNameClaim(identity);

            Assert.AreEqual(1, identity.FindAll(ClaimTypes.Name).Count());
            Assert.AreEqual("Jane Doe", identity.Name);
        }
    }
}
