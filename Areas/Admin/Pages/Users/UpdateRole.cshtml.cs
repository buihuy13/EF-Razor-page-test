
using EFRazor.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System.ComponentModel;

namespace Users
{
    public class UpdateRoleModel : PageModel
    {
        private readonly UserManager<appUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly BlogContext _context;
        public UpdateRoleModel(UserManager<appUser> userManager, RoleManager<IdentityRole> roleManager, BlogContext context)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            this._context = context;
        }
        [TempData]
        public string? StatusMessage { get; set; }
        public List<IdentityRole>? roles { get; set; }
        public appUser? user { get; set; }

        [BindProperty]
        [DisplayName("Roles Selection")]
        public string[]? roleNames { get; set; }
        public SelectList allRoles { get; set; }
        public List<IdentityRoleClaim<string>> roleClaims { get; set; }
        public List<IdentityUserClaim<string>> userClaims { get; set; }

        async Task GetClaims(string id)
        {
            var listRoles = from r in _context.Roles
                            join ur in _context.UserRoles on r.Id equals ur.RoleId
                            where ur.UserId == id
                            select r;

            roleClaims = await (from c in _context.RoleClaims
                               join r in listRoles on c.RoleId equals r.Id
                               select c).ToListAsync();

            userClaims = await (from c in _context.UserClaims
                         where c.UserId == id
                         select c).ToListAsync();
        }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            roles = await roleManager.Roles.OrderBy(r => r.Name).ToListAsync();
            user = await userManager.FindByIdAsync(id);

            if (user == null)
            {
                StatusMessage = "Cannot find user with such " + id;
                return RedirectToPage("./Index");
            }

            roleNames = (await userManager.GetRolesAsync(user)).ToArray();

            allRoles = new SelectList(await roleManager.Roles.Select(r => r.Name).ToListAsync());
            await GetClaims(id);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string id)
        {
            user = await userManager.FindByIdAsync(id);
            roles = await roleManager.Roles.ToListAsync();
            if (user == null || roles == null)
            {
                StatusMessage = "Cannot find user with such " + id;
                return RedirectToPage("./Index");
            }

            var OldRoleName = (await userManager.GetRolesAsync(user)).ToArray();

            var deleteRole = OldRoleName.Where(r => !roleNames.Contains(r));

            var addNewRoles = roleNames.Where(r => !OldRoleName.Contains(r));

            var result = await userManager.AddToRolesAsync(user, addNewRoles);

            var result2 = await userManager.RemoveFromRolesAsync(user, deleteRole);

            if (!result.Succeeded || !result2.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                foreach (var error in result2.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return Page();
            }
            await GetClaims(id);
            return RedirectToPage("./Index");
        }
    }
}