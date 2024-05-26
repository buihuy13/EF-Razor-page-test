using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using EFRazor.Models;

namespace EFRazor.Pages.MyBLog
{
    public class IndexModel : PageModel
    {
        private readonly BlogContext _context;

        public IndexModel(BlogContext context)
        {
            _context = context;
        }

        public IList<Article> Article { get; set; } = default!;

        public const int ITEMS_PER_PAGE = 10;

        [BindProperty(SupportsGet = true, Name = "p")]
        public int currentPage { set; get; }

        public int countPages { set; get; }
        public async Task OnGetAsync([FromQuery]string SearchString)
        {
            if (_context.articles != null)
            {
                int totalArticle = await _context.articles.CountAsync();
                countPages = totalArticle / ITEMS_PER_PAGE;
                if (totalArticle % ITEMS_PER_PAGE != 0)
                {
                    countPages++;
                }
                if (currentPage < 1)
                {
                    currentPage = 1;
                }
                if (currentPage > countPages)
                {
                    currentPage = countPages;
                }
                var posts = (from p in _context.articles
                             orderby p.Created descending
                             select p).Skip((currentPage - 1) * 10).Take(ITEMS_PER_PAGE);

                if (string.IsNullOrEmpty(SearchString) == false)
                {
                    var searchposts = posts.Where(p => p.Title.Contains(SearchString));
                    Article = await searchposts.ToListAsync();
                }

                else
                {
                    Article = await posts.ToListAsync();
                }    
            }

        }
    }
}
