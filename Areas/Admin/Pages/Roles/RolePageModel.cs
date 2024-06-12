using EFRazor.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Roles
{
    public class RolePageModel : PageModel
    {
        protected readonly RoleManager<IdentityRole> _RoleManager;

        protected readonly BlogContext _context;
        [TempData]
        public string? StatusMessage { get; set; }
        public RolePageModel(RoleManager<IdentityRole> roleManager, BlogContext context)
        {
            _RoleManager = roleManager;
            _context = context;
        }
    }
}