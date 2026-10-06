using System.Collections.Generic;

namespace MyApp.Web.Models
{
    /// <summary>
    /// A page of site content. Served both as a Razor view and through the content API.
    /// </summary>
    public class ContentPage
    {
        public string Slug { get; set; }

        public string Title { get; set; }

        public string Summary { get; set; }

        public IList<string> Paragraphs { get; set; } = new List<string>();
    }
}
