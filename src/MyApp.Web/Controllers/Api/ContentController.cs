using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using MyApp.Web.Models;
using MyApp.Web.Services;

namespace MyApp.Web.Controllers.Api
{
    [RoutePrefix("api/content")]
    public class ContentController : ApiController
    {
        private readonly IContentService _content;

        public ContentController()
            : this(new ContentService())
        {
        }

        public ContentController(IContentService content)
        {
            _content = content;
        }

        /// <summary>GET api/content — summaries of every page.</summary>
        [HttpGet]
        [Route("")]
        public IEnumerable<ContentPageSummary> GetAll()
        {
            return _content.GetAll()
                .Select(p => new ContentPageSummary { Slug = p.Slug, Title = p.Title, Summary = p.Summary })
                .ToList();
        }

        /// <summary>GET api/content/{slug} — a single page.</summary>
        [HttpGet]
        [Route("{slug}", Name = "GetContentPage")]
        public IHttpActionResult Get(string slug)
        {
            var page = _content.GetBySlug(slug);
            if (page == null)
            {
                return NotFound();
            }

            return Ok(page);
        }
    }

    public class ContentPageSummary
    {
        public string Slug { get; set; }

        public string Title { get; set; }

        public string Summary { get; set; }
    }
}
