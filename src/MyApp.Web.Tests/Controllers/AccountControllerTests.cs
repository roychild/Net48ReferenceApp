using System;
using System.Collections.Specialized;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MyApp.Web.Controllers;
using MyApp.Web.Models;

namespace MyApp.Web.Tests.Controllers
{
    [TestClass]
    public class AccountControllerTests
    {
        [TestMethod]
        [DataRow("/home/about", "/home/about")]
        [DataRow("/account/profile?tab=1", "/account/profile?tab=1")]
        [DataRow("https://evil.example.com/", "/")]
        [DataRow("//evil.example.com/", "/")]
        [DataRow("/\\evil.example.com/", "/")]
        [DataRow("", "/")]
        [DataRow(null, "/")]
        public void SafeReturnUrl_OnlyAllowsLocalUrls(string requested, string expected)
        {
            var controller = CreateController(TestPrincipals.Anonymous);

            Assert.AreEqual(expected, controller.SafeReturnUrl(requested));
        }

        [TestMethod]
        public void Login_RedirectsAuthenticatedUserToReturnUrl()
        {
            var controller = CreateController(TestPrincipals.Saml("jdoe", "https://idp.example.com"));

            var result = controller.Login("/home/about") as RedirectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("/home/about", result.Url);
        }

        [TestMethod]
        public void Profile_RendersClaimsOfCurrentUser()
        {
            var controller = CreateController(TestPrincipals.Saml("jdoe", "https://idp.example.com"));

            var result = controller.ShowProfile() as ViewResult;

            var profile = result?.Model as UserProfile;
            Assert.IsNotNull(profile);
            Assert.AreEqual("jdoe", profile.NameIdentifier);
        }

        [TestMethod]
        public void Profile_RequiresAuthorization()
        {
            var method = typeof(AccountController).GetMethod(nameof(AccountController.ShowProfile));

            Assert.IsTrue(Attribute.IsDefined(method, typeof(AuthorizeAttribute)));
        }

        [TestMethod]
        public void Logout_RequiresPostWithAntiForgeryToken()
        {
            var method = typeof(AccountController).GetMethod(nameof(AccountController.Logout));

            Assert.IsTrue(Attribute.IsDefined(method, typeof(HttpPostAttribute)));
            Assert.IsTrue(Attribute.IsDefined(method, typeof(ValidateAntiForgeryTokenAttribute)));
        }

        private static AccountController CreateController(System.Security.Principal.IPrincipal user)
        {
            var request = new Mock<HttpRequestBase>();
            request.SetupGet(r => r.ApplicationPath).Returns("/");
            request.SetupGet(r => r.Url).Returns(new Uri("https://localhost:44300/"));
            request.SetupGet(r => r.ServerVariables).Returns(new NameValueCollection());

            var response = new Mock<HttpResponseBase>();
            response.Setup(r => r.ApplyAppPathModifier(It.IsAny<string>())).Returns<string>(s => s);

            var httpContext = new Mock<HttpContextBase>();
            httpContext.SetupGet(c => c.Request).Returns(request.Object);
            httpContext.SetupGet(c => c.Response).Returns(response.Object);
            httpContext.SetupGet(c => c.User).Returns(user);
            httpContext.SetupGet(c => c.Items).Returns(new System.Collections.Hashtable());

            var routes = new RouteCollection();
            RouteConfig.RegisterRoutes(routes);

            var requestContext = new RequestContext(httpContext.Object, new RouteData());
            var controller = new AccountController
            {
                Url = new UrlHelper(requestContext, routes),
            };
            controller.ControllerContext = new ControllerContext(requestContext, controller);
            return controller;
        }
    }
}
