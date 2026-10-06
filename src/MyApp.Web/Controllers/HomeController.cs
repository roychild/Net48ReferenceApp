using System.Web.Mvc;
using MyApp.Web.Services;

namespace MyApp.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IContentService _content;

        public HomeController()
            : this(new ContentService())
        {
        }

        public HomeController(IContentService content)
        {
            _content = content;
        }

        public ActionResult Index()
        {
            return ContentView(ContentService.HomeSlug);
        }

        public ActionResult About()
        {
            return ContentView(ContentService.AboutSlug);
        }

        public ActionResult Services()
        {
            return ContentView(ContentService.ServicesSlug);
        }

        public ActionResult Contact()
        {
            return ContentView(ContentService.ContactSlug);
        }

        private ActionResult ContentView(string slug)
        {
            var page = _content.GetBySlug(slug);
            if (page == null)
            {
                return HttpNotFound();
            }

            ViewBag.Title = page.Title;
            return View(page);
        }
    }
}
