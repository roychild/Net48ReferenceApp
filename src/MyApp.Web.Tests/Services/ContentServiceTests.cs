using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyApp.Web.Services;

namespace MyApp.Web.Tests.Services
{
    [TestClass]
    public class ContentServiceTests
    {
        private readonly ContentService _service = new ContentService();

        [TestMethod]
        public void GetAll_ReturnsEveryNavigationPage()
        {
            var slugs = _service.GetAll().Select(p => p.Slug).ToList();

            CollectionAssert.AreEquivalent(
                new[] { ContentService.HomeSlug, ContentService.AboutSlug, ContentService.ServicesSlug, ContentService.ContactSlug },
                slugs);
        }

        [TestMethod]
        public void GetAll_PagesHaveTitleAndBody()
        {
            foreach (var page in _service.GetAll())
            {
                Assert.IsFalse(string.IsNullOrWhiteSpace(page.Title), page.Slug);
                Assert.IsTrue(page.Paragraphs.Count > 0, page.Slug);
            }
        }

        [TestMethod]
        [DataRow("about")]
        [DataRow("ABOUT")]
        [DataRow(" about ")]
        public void GetBySlug_IsCaseAndWhitespaceInsensitive(string slug)
        {
            Assert.AreEqual(ContentService.AboutSlug, _service.GetBySlug(slug)?.Slug);
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("missing")]
        public void GetBySlug_ReturnsNullForUnknownSlug(string slug)
        {
            Assert.IsNull(_service.GetBySlug(slug));
        }
    }
}
