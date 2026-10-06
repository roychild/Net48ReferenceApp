using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using MyApp.Web.Models;

namespace MyApp.Web.Services
{
    public static class UserProfileFactory
    {
        /// <summary>Builds a profile from the authenticated principal, or returns <c>null</c> for anonymous users.</summary>
        public static UserProfile Create(IPrincipal principal)
        {
            var identity = principal?.Identity as ClaimsIdentity;
            if (identity == null || !identity.IsAuthenticated)
            {
                return null;
            }

            var nameId = identity.FindFirst(ClaimTypes.NameIdentifier);

            return new UserProfile
            {
                Name = identity.Name ?? nameId?.Value,
                NameIdentifier = nameId?.Value,
                Issuer = nameId?.Issuer,
                Claims = identity.Claims
                    .Select(c => new ClaimInfo { Type = c.Type, Value = c.Value })
                    .ToList(),
            };
        }
    }
}
