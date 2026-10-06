using System.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyApp.Web.Security;

namespace MyApp.Web.Tests.Security
{
    [TestClass]
    public class Saml2SettingsTests
    {
        [TestMethod]
        public void FromNameValueCollection_ReadsValues()
        {
            var values = TestSettings.Values();
            values["Saml2:SigningCertificateThumbprint"] = " ABC123 ";
            values["Saml2:AllowUnsolicitedAuthnResponse"] = "true";
            values["Saml2:ModulePath"] = "/AuthServices";

            var settings = Saml2Settings.FromNameValueCollection(values);

            Assert.AreEqual("https://localhost:44300/Saml2", settings.SpEntityId);
            Assert.AreEqual("https://localhost:44300/", settings.ReturnUrl);
            Assert.AreEqual("https://idp.example.com/metadata", settings.IdpEntityId);
            Assert.AreEqual("ABC123", settings.SigningCertificateThumbprint);
            Assert.IsTrue(settings.AllowUnsolicitedAuthnResponse);
            Assert.IsFalse(settings.LoadIdpMetadata);
            Assert.AreEqual("/AuthServices", settings.ModulePath);
        }

        [TestMethod]
        public void FromNameValueCollection_AppliesDefaults()
        {
            var values = TestSettings.Values();
            values.Remove("Saml2:LoadIdpMetadata");

            var settings = Saml2Settings.FromNameValueCollection(values);

            Assert.AreEqual("/Saml2", settings.ModulePath);
            Assert.IsTrue(settings.LoadIdpMetadata);
            Assert.IsFalse(settings.AllowUnsolicitedAuthnResponse);
            Assert.IsNull(settings.SigningCertificateThumbprint);
            Assert.AreEqual(settings.IdpEntityId, settings.IdpMetadataUrl, "Metadata URL defaults to the IdP entity ID.");
        }

        [TestMethod]
        [DataRow("Saml2:SpEntityId")]
        [DataRow("Saml2:ReturnUrl")]
        [DataRow("Saml2:IdpEntityId")]
        public void FromNameValueCollection_ThrowsWhenRequiredValueMissing(string key)
        {
            var values = TestSettings.Values();
            values[key] = " ";

            var ex = Assert.ThrowsExactly<ConfigurationErrorsException>(() => Saml2Settings.FromNameValueCollection(values));
            StringAssert.Contains(ex.Message, key);
        }

        [TestMethod]
        public void FromNameValueCollection_ThrowsOnInvalidBoolean()
        {
            var values = TestSettings.Values();
            values["Saml2:LoadIdpMetadata"] = "yes";

            Assert.ThrowsExactly<ConfigurationErrorsException>(() => Saml2Settings.FromNameValueCollection(values));
        }
    }
}
