using System.Web.Http;
using MyApp.Web.Services;

namespace MyApp.Web.Controllers.Api
{
    [Authorize]
    [RoutePrefix("api/profile")]
    public class ProfileController : ApiController
    {
        /// <summary>GET api/profile — the signed-in user's profile and claims (401 when anonymous).</summary>
        [HttpGet]
        [Route("")]
        public IHttpActionResult Get()
        {
            var profile = UserProfileFactory.Create(User);
            if (profile == null)
            {
                return Unauthorized();
            }

            return Ok(profile);
        }
    }
}
