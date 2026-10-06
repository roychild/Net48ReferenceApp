using System.Linq;
using System.Web.Http.Results;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyApp.Web.Controllers.Api;
using MyApp.Web.Models;
using MyApp.Web.Services;

namespace MyApp.Web.Tests.Controllers
{
    [TestClass]
    public class ContentApiControllerTests
    {
        private readonly ContentController _controller = new ContentController(new ContentService());

        [TestMethod]
        public void GetAll_ReturnsSummaryForEveryPage()
        {
            var result = _controller.GetAll().ToList();

            Assert.AreEqual(new ContentService().GetAll().Count, result.Count);
            Assert.IsTrue(result.All(p => !string.IsNullOrEmpty(p.Slug) && !string.IsNullOrEmpty(p.Title)));
        }

        [TestMethod]
        public void Get_ReturnsPage()
        {
            var result = _controller.Get("about") as OkNegotiatedContentResult<ContentPage>;

            Assert.IsNotNull(result);
            Assert.AreEqual(ContentService.AboutSlug, result.Content.Slug);
        }

        [TestMethod]
        public void Get_ReturnsNotFoundForUnknownSlug()
        {
            Assert.IsInstanceOfType(_controller.Get("nope"), typeof(NotFoundResult));
        }
    }
}
