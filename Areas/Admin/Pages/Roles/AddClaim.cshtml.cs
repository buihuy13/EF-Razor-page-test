
using EFRazor.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Roles;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

public class AddClaimModel : RolePageModel
{
    public AddClaimModel(RoleManager<IdentityRole> roleManager, BlogContext context) : base(roleManager, context)
    {
    }

    [BindProperty]
    public InputModel Input { get; set; }
    public class InputModel
    {
        [Required]
        [StringLength(256, MinimumLength =3)]
        [DisplayName("Claim Type")]
        public string ClaimType { get; set; }

        [Required]
        [StringLength(256, MinimumLength =3)]
        [DisplayName("Claim Value")]
        public string ClaimValue { get; set;}
    }
    public IdentityRole? role {  get; set; }
    public async Task<IActionResult> OnGetAsync(string roleid)
    {
        role = await _RoleManager.FindByIdAsync(roleid);
        if (role==null)
        {
            return NotFound("No Such Role");
        }
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(string roleid)
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }
        role = await _RoleManager.FindByIdAsync(roleid);
        if (role==null)
        {
            return NotFound("No Such Role");
        }

        if ((await _RoleManager.GetClaimsAsync(role)).Any(c => c.Type == Input.ClaimType && c.Value == Input.ClaimValue) == true)
        {
            StatusMessage = "Error, Cannot create existed claim";
            return Page();
        }
        Claim newClaim = new Claim(Input.ClaimType, Input.ClaimValue);
        var result = await _RoleManager.AddClaimAsync(role, newClaim);

        if (!result.Succeeded)
        {
            foreach(var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return Page();
        }
        StatusMessage = "Them Claim thanh cong";
        return RedirectToPage("./Edit", new { roleid = role.Id});
    }
}