using System;
using System.Web.Http;
using System.Web.Http.Results;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyApp.Web.Controllers.Api;
using MyApp.Web.Models;

namespace MyApp.Web.Tests.Controllers
{
    [TestClass]
    public class ProfileApiControllerTests
    {
        [TestMethod]
        public void Controller_RequiresAuthorization()
        {
            Assert.IsTrue(Attribute.IsDefined(typeof(ProfileController), typeof(AuthorizeAttribute)));
        }

        [TestMethod]
        public void Get_ReturnsProfileOfCurrentUser()
        {
            var controller = new ProfileController { User = TestPrincipals.Saml("jdoe", "https://idp.example.com") };

            var result = controller.Get() as OkNegotiatedContentResult<UserProfile>;

            Assert.IsNotNull(result);
            Assert.AreEqual("jdoe", result.Content.NameIdentifier);
        }

        [TestMethod]
        public void Get_ReturnsUnauthorizedForAnonymousUser()
        {
            var controller = new ProfileController { User = TestPrincipals.Anonymous };

            Assert.IsInstanceOfType(controller.Get(), typeof(UnauthorizedResult));
        }
    }
}
