using System.Web;
using System.Web.Mvc;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using MyApp.Web.Security;
using MyApp.Web.Services;

namespace MyApp.Web.Controllers
{
    public class AccountController : Controller
    {
        private IAuthenticationManager Authentication => HttpContext.GetOwinContext().Authentication;

        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            var target = SafeReturnUrl(returnUrl);

            if (User?.Identity?.IsAuthenticated == true)
            {
                return Redirect(target);
            }

            // Hands off to the Sustainsys.Saml2 middleware, which redirects to the IdP.
            Authentication.Challenge(new AuthenticationProperties { RedirectUri = target }, Saml2OptionsFactory.AuthenticationType);
            return new HttpUnauthorizedResult();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Logout()
        {
            // Signing out of Saml2 as well sends a single-logout request to the IdP when the
            // IdP supports it and a signing certificate is configured; otherwise it is a local logout.
            Authentication.SignOut(
                new AuthenticationProperties { RedirectUri = Url.Action("Index", "Home") },
                CookieAuthenticationDefaults.AuthenticationType,
                Saml2OptionsFactory.AuthenticationType);

            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        [ActionName("Profile")]
        public ActionResult ShowProfile()
        {
            ViewBag.Title = "Your profile";
            return View(UserProfileFactory.Create(User));
        }

        internal string SafeReturnUrl(string returnUrl)
        {
            return !string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl)
                ? returnUrl
                : Url.Action("Index", "Home");
        }
    }
}
