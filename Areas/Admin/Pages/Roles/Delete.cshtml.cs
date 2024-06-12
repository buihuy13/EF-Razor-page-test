
using EFRazor.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Roles
{
    public class DeleteModel : RolePageModel
    {
        public DeleteModel(RoleManager<IdentityRole> roleManager, BlogContext context) : base(roleManager, context)
        {
        }

        public void OnGet()
        {

        }

        public async Task<IActionResult> OnPost(string roleid)
        {
            var role = await _RoleManager.FindByIdAsync(roleid);
            if (role != null)
            {
                if (!ModelState.IsValid)
                {
                    return Page();
                }

                var result = await _RoleManager.DeleteAsync(role);
                if (!result.Succeeded) 
                { 
                    foreach(var error in result.Errors) 
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return Page();
                }
                StatusMessage = "Delete Success";
            }

            return RedirectToPage("./Index");
        }
    }
}
