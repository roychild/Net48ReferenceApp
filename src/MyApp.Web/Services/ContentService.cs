using System;
using System.Collections.Generic;
using System.Linq;
using MyApp.Web.Models;

namespace MyApp.Web.Services
{
    /// <summary>
    /// In-memory content store. Swap for a CMS or database-backed implementation as needed.
    /// </summary>
    public class ContentService : IContentService
    {
        public const string HomeSlug = "home";
        public const string AboutSlug = "about";
        public const string ServicesSlug = "services";
        public const string ContactSlug = "contact";

        private static readonly IReadOnlyList<ContentPage> Pages = new List<ContentPage>
        {
            new ContentPage
            {
                Slug = HomeSlug,
                Title = "Welcome to MyApp",
                Summary = "A reference ASP.NET application for .NET Framework 4.8.1 with SAML single sign-on.",
                Paragraphs =
                {
                    "MyApp combines ASP.NET MVC Razor views for server-rendered pages with ASP.NET Web API for JSON endpoints, hosted together in one OWIN pipeline.",
                    "Sign in with your organisation's identity provider to see your profile and the claims it sent.",
                },
            },
            new ContentPage
            {
                Slug = AboutSlug,
                Title = "About",
                Summary = "What this application is and how it is put together.",
                Paragraphs =
                {
                    "MyApp.Web is a starting point for line-of-business web applications that must run on the .NET Framework.",
                    "Authentication is handled by Sustainsys.Saml2 running as OWIN middleware. After a successful SAML response the user is signed in with an application cookie.",
                },
            },
            new ContentPage
            {
                Slug = ServicesSlug,
                Title = "Services",
                Summary = "The capabilities this reference application demonstrates.",
                Paragraphs =
                {
                    "Server-rendered pages: Razor views with a shared layout.",
                    "JSON API: attribute-routed Web API controllers under /api.",
                    "Single sign-on: SAML 2.0 service provider with metadata published at /Saml2.",
                },
            },
            new ContentPage
            {
                Slug = ContactSlug,
                Title = "Contact",
                Summary = "How to get in touch.",
                Paragraphs =
                {
                    "Email: support@example.com",
                    "Phone: +1 (555) 010-0000",
                },
            },
        };

        public IReadOnlyList<ContentPage> GetAll()
        {
            return Pages;
        }

        public ContentPage GetBySlug(string slug)
        {
            if (string.IsNullOrWhiteSpace(slug))
            {
                return null;
            }

            return Pages.FirstOrDefault(p => string.Equals(p.Slug, slug.Trim(), StringComparison.OrdinalIgnoreCase));
        }
    }
}
