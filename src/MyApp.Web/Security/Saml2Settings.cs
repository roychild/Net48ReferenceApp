using System;
using System.Collections.Specialized;
using System.Configuration;

namespace MyApp.Web.Security
{
    /// <summary>
    /// SAML2 service provider / identity provider settings, read from the
    /// <c>Saml2:*</c> keys in the <c>appSettings</c> section of Web.config.
    /// </summary>
    public class Saml2Settings
    {
        public const string KeyPrefix = "Saml2:";

        /// <summary>Entity ID of this application (the service provider).</summary>
        public string SpEntityId { get; set; }

        /// <summary>Where users land after signing in when no return URL was requested.</summary>
        public string ReturnUrl { get; set; }

        /// <summary>Path the Saml2 middleware listens on (metadata, ACS, logout).</summary>
        public string ModulePath { get; set; } = "/Saml2";

        /// <summary>Entity ID of the identity provider.</summary>
        public string IdpEntityId { get; set; }

        /// <summary>URL the identity provider's metadata is loaded from.</summary>
        public string IdpMetadataUrl { get; set; }

        /// <summary>Whether to download and periodically refresh the IdP metadata.</summary>
        public bool LoadIdpMetadata { get; set; } = true;

        /// <summary>Allow responses the SP did not request (IdP-initiated sign-on).</summary>
        public bool AllowUnsolicitedAuthnResponse { get; set; }

        /// <summary>
        /// Optional thumbprint of a certificate (with private key) in LocalMachine\My used to sign
        /// requests. Required for single logout.
        /// </summary>
        public string SigningCertificateThumbprint { get; set; }

        public static Saml2Settings FromAppSettings()
        {
            return FromNameValueCollection(ConfigurationManager.AppSettings);
        }

        public static Saml2Settings FromNameValueCollection(NameValueCollection values)
        {
            if (values == null)
            {
                throw new ArgumentNullException(nameof(values));
            }

            var settings = new Saml2Settings
            {
                SpEntityId = Required(values, "SpEntityId"),
                ReturnUrl = Required(values, "ReturnUrl"),
                IdpEntityId = Required(values, "IdpEntityId"),
                IdpMetadataUrl = Optional(values, "IdpMetadataUrl"),
                SigningCertificateThumbprint = Optional(values, "SigningCertificateThumbprint"),
                AllowUnsolicitedAuthnResponse = Bool(values, "AllowUnsolicitedAuthnResponse", false),
                LoadIdpMetadata = Bool(values, "LoadIdpMetadata", true),
            };

            var modulePath = Optional(values, "ModulePath");
            if (modulePath != null)
            {
                settings.ModulePath = modulePath;
            }

            if (settings.LoadIdpMetadata && settings.IdpMetadataUrl == null)
            {
                settings.IdpMetadataUrl = settings.IdpEntityId;
            }

            return settings;
        }

        private static string Optional(NameValueCollection values, string key)
        {
            var value = values[KeyPrefix + key];
            return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
        }

        private static string Required(NameValueCollection values, string key)
        {
            var value = Optional(values, key);
            if (value == null)
            {
                throw new ConfigurationErrorsException($"Missing required appSetting '{KeyPrefix}{key}'.");
            }

            return value;
        }

        private static bool Bool(NameValueCollection values, string key, bool defaultValue)
        {
            var value = Optional(values, key);
            if (value == null)
            {
                return defaultValue;
            }

            if (!bool.TryParse(value, out var result))
            {
                throw new ConfigurationErrorsException($"appSetting '{KeyPrefix}{key}' must be 'true' or 'false'.");
            }

            return result;
        }
    }
}
