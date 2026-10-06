using System.Collections.Generic;
using MyApp.Web.Models;

namespace MyApp.Web.Services
{
    public interface IContentService
    {
        IReadOnlyList<ContentPage> GetAll();

        /// <summary>Returns the page with the given slug, or <c>null</c> if there is none.</summary>
        ContentPage GetBySlug(string slug);
    }
}
