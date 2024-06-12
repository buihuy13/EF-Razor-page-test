
using EFRazor.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Roles
{
    public class CreateModel : RolePageModel
    {
        public CreateModel(RoleManager<IdentityRole> roleManager, BlogContext context) : base(roleManager, context)
        {
        }

        [BindProperty]
        public InputModel Input { get; set; }
        public class InputModel
        {


            [StringLength(256)]
            [DisplayName("Role's Name")]
            [Required]
            public string Name { get; set; }
        }
        public void OnGet()
        {

        }

        public async Task<IActionResult> OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            var newRole = new IdentityRole(Input.Name);
            var result = await _RoleManager.CreateAsync(newRole);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return Page();
            }
            StatusMessage = "Success Created";

            return RedirectToPage("./Index");
        }
    }
}