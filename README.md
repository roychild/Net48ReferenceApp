# Net48ReferenceApp

A reference ASP.NET web application for **.NET Framework 4.8.1** that combines:

- **ASP.NET MVC 5 with Razor views** for server-rendered pages (a landing page plus About, Services and Contact pages)
- **ASP.NET Web API 2** for JSON endpoints
- **SAML 2.0 single sign-on** using [Sustainsys.Saml2](https://github.com/Sustainsys/Saml2) (`Sustainsys.Saml2` + `Sustainsys.Saml2.Owin`) on an OWIN pipeline with cookie authentication

> ASP.NET Core "Razor Pages" do not exist on .NET Framework. On 4.8.1, Razor is used through MVC 5 views, which is what this app does.

## Solution layout

```
MyApp.Web.sln
├── src/MyApp.Web/                  ASP.NET Web Application (root namespace MyApp.Web)
│   ├── App_Start/                  MVC routes, Web API config, global filters
│   ├── Controllers/                MVC controllers (Home, Account)
│   ├── Controllers/Api/            Web API controllers (content, profile)
│   ├── Models/                     View / API models
│   ├── Security/                   SAML2 settings and Sustainsys options factory
│   ├── Services/                   Content service, user-profile mapping
│   ├── Views/                      Razor views and shared layout
│   ├── Startup.cs                  OWIN startup: cookie auth + Saml2 middleware
│   └── Web.config                  App settings (incl. Saml2:*), binding redirects
└── tests/MyApp.Web.Tests/          MSTest unit tests
```

## Pages and endpoints

| URL | Description | Auth |
| --- | --- | --- |
| `/` | Landing page | Anonymous |
| `/home/about`, `/home/services`, `/home/contact` | Content pages | Anonymous |
| `/account/login?returnUrl=…` | Starts SAML sign-in at the IdP | Anonymous |
| `/account/logout` (POST) | Signs out (and single logout when configured) | Signed in |
| `/account/profile` | The signed-in user's claims | Signed in |
| `GET /api/content` | Page summaries as JSON | Anonymous |
| `GET /api/content/{slug}` | A single page as JSON | Anonymous |
| `GET /api/profile` | Current user's claims as JSON (401 when anonymous) | Signed in |
| `/Saml2` | SP metadata (ACS at `/Saml2/Acs`) | Anonymous |

## Getting started

Requirements: Windows, Visual Studio 2022 with the *ASP.NET and web development* workload, and the .NET Framework 4.8.1 developer pack (or rely on the `Microsoft.NETFramework.ReferenceAssemblies` package the projects reference).

1. Open `MyApp.Web.sln` in Visual Studio. NuGet packages restore automatically (PackageReference).
2. Press **F5**. The site runs on IIS Express at `https://localhost:44300/`.
3. Click **Sign in**. Out of the box the app uses the public [Sustainsys stub IdP](https://stubidp.sustainsys.com/), which lets you sign in as any user without a password, so you can try the whole flow without setting up an IdP.

Run the tests from Test Explorer, or from the command line:

```
msbuild MyApp.Web.sln /restore
vstest.console tests\MyApp.Web.Tests\bin\Debug\net481\MyApp.Web.Tests.dll
```

## Configuring SAML

All SAML settings are `appSettings` in `src/MyApp.Web/Web.config`, read by `Security/Saml2Settings.cs` and applied in `Security/Saml2OptionsFactory.cs`:

| Key | Purpose |
| --- | --- |
| `Saml2:SpEntityId` | This app's entity ID, as registered with the IdP |
| `Saml2:ReturnUrl` | Default landing page after sign-in |
| `Saml2:ModulePath` | Path the Saml2 middleware serves (default `/Saml2`) |
| `Saml2:IdpEntityId` | The IdP's entity ID |
| `Saml2:IdpMetadataUrl` | Where to load IdP metadata (defaults to the IdP entity ID) |
| `Saml2:LoadIdpMetadata` | Load and refresh IdP metadata (default `true`) |
| `Saml2:AllowUnsolicitedAuthnResponse` | Accept IdP-initiated sign-on (default `false`) |
| `Saml2:SigningCertificateThumbprint` | Optional certificate in `LocalMachine\My` used to sign requests; needed for single logout |

To connect a real IdP:

1. Give the IdP this app's metadata URL (`https://<host>/Saml2`) or its entity ID and ACS URL (`https://<host>/Saml2/Acs`).
2. Set `Saml2:IdpEntityId` and `Saml2:IdpMetadataUrl` to the IdP's values. `Web.Release.config` has a commented transform for production values.
3. For single logout, install a signing certificate with a private key in `LocalMachine\My`, grant the app pool identity read access to the key, and set `Saml2:SigningCertificateThumbprint`.

After a successful SAML response the user is signed in with an application cookie (`MyApp.Auth`, HTTPS only, 8-hour sliding expiration). If the IdP sends no name attribute, the SAML NameID is used as `User.Identity.Name`. Unauthenticated requests to MVC pages are redirected to the IdP. Requests under `/api` get a `401` instead.
