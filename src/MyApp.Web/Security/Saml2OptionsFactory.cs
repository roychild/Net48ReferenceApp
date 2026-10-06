using System;
using System.Configuration;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using Sustainsys.Saml2;
using Sustainsys.Saml2.Configuration;
using Sustainsys.Saml2.Metadata;
using Sustainsys.Saml2.Owin;

namespace MyApp.Web.Security
{
    public static class Saml2OptionsFactory
    {
        public const string AuthenticationType = "Saml2";

        public static Saml2AuthenticationOptions Create(Saml2Settings settings)
        {
            return Create(settings, LoadCertificateFromStore);
        }

        internal static Saml2AuthenticationOptions Create(Saml2Settings settings, Func<string, X509Certificate2> loadCertificate)
        {
            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            // false: don't read the legacy <sustainsys.saml2> config section; everything is set in code.
            var options = new Saml2AuthenticationOptions(false)
            {
                SPOptions = CreateSPOptions(settings, loadCertificate),
            };

            var idp = new IdentityProvider(new EntityId(settings.IdpEntityId), options.SPOptions)
            {
                AllowUnsolicitedAuthnResponse = settings.AllowUnsolicitedAuthnResponse,
            };

            if (settings.LoadIdpMetadata)
            {
                idp.MetadataLocation = settings.IdpMetadataUrl;
                idp.LoadMetadata = true;
            }

            options.IdentityProviders.Add(idp);

            options.Notifications.AcsCommandResultCreated = (result, response) =>
            {
                if (result.Principal?.Identity is ClaimsIdentity identity)
                {
                    EnsureNameClaim(identity);
                }
            };

            return options;
        }

        internal static SPOptions CreateSPOptions(Saml2Settings settings, Func<string, X509Certificate2> loadCertificate)
        {
            var spOptions = new SPOptions
            {
                EntityId = new EntityId(settings.SpEntityId),
                ReturnUrl = new Uri(settings.ReturnUrl),
                ModulePath = settings.ModulePath,
            };

            if (settings.SigningCertificateThumbprint != null)
            {
                spOptions.ServiceCertificates.Add(loadCertificate(settings.SigningCertificateThumbprint));
            }

            return spOptions;
        }

        /// <summary>
        /// Most IdPs identify the user only by NameID. Copy it into a Name claim so
        /// <c>User.Identity.Name</c> is populated when no explicit name attribute was sent.
        /// </summary>
        internal static void EnsureNameClaim(ClaimsIdentity identity)
        {
            if (identity.FindFirst(identity.NameClaimType) != null)
            {
                return;
            }

            var nameId = identity.FindFirst(ClaimTypes.NameIdentifier);
            if (nameId != null)
            {
                identity.AddClaim(new Claim(identity.NameClaimType, nameId.Value, nameId.ValueType, nameId.Issuer));
            }
        }

        private static X509Certificate2 LoadCertificateFromStore(string thumbprint)
        {
            using (var store = new X509Store(StoreName.My, StoreLocation.LocalMachine))
            {
                store.Open(OpenFlags.ReadOnly);
                var matches = store.Certificates.Find(X509FindType.FindByThumbprint, thumbprint, validOnly: false);
                if (matches.Count == 0)
                {
                    throw new ConfigurationErrorsException($"SAML signing certificate '{thumbprint}' was not found in LocalMachine\\My.");
                }

                return matches[0];
            }
        }
    }
}
