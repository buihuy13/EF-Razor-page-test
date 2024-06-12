
using EFRazor.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace Roles
{
    [Authorize(Roles = "Admin")]
    public class IndexModel : RolePageModel
    {
        public IndexModel(RoleManager<IdentityRole> roleManager, BlogContext context) : base(roleManager, context)
        {
        }
        public List<IdentityRole>? roles { get; set; }
        public async Task OnGet()
        {
            roles = await _RoleManager.Roles.ToListAsync();
        }
    }
}