using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using EFRazor.Models;

namespace EFRazor.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly BlogContext blogContext;
        public IndexModel(ILogger<IndexModel> logger, BlogContext blogContext)
        {
            _logger = logger;
            this.blogContext = blogContext;
        }

        public void OnGet()
        {
            var posts = (from p in blogContext.articles
                        orderby p.Created descending
                        select p).ToList();

            ViewData["Posts"] = posts;
        }
    }
}