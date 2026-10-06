using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MyApp.Web.Controllers;
using MyApp.Web.Models;
using MyApp.Web.Services;

namespace MyApp.Web.Tests.Controllers
{
    [TestClass]
    public class HomeControllerTests
    {
        [TestMethod]
        public void Index_RendersHomePage()
        {
            AssertRendersPage(c => c.Index(), ContentService.HomeSlug);
        }

        [TestMethod]
        public void About_RendersAboutPage()
        {
            AssertRendersPage(c => c.About(), ContentService.AboutSlug);
        }

        [TestMethod]
        public void Services_RendersServicesPage()
        {
            AssertRendersPage(c => c.Services(), ContentService.ServicesSlug);
        }

        [TestMethod]
        public void Contact_RendersContactPage()
        {
            AssertRendersPage(c => c.Contact(), ContentService.ContactSlug);
        }

        [TestMethod]
        public void Action_ReturnsNotFoundWhenContentMissing()
        {
            var content = new Mock<IContentService>();
            var controller = new HomeController(content.Object);

            Assert.IsInstanceOfType(controller.About(), typeof(HttpNotFoundResult));
        }

        private static void AssertRendersPage(System.Func<HomeController, ActionResult> action, string slug)
        {
            var controller = new HomeController(new ContentService());

            var result = action(controller) as ViewResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(string.Empty, result.ViewName, "Should use the action's default view.");
            var page = result.Model as ContentPage;
            Assert.IsNotNull(page);
            Assert.AreEqual(slug, page.Slug);
            Assert.AreEqual(page.Title, controller.ViewBag.Title);
        }
    }
}
