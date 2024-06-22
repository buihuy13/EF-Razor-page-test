
using EFRazor.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Security.Claims;

public class EditUserRoleClaimsModel : PageModel
{
    private readonly UserManager<appUser> userManager;
    private readonly RoleManager<IdentityRole> roleManager;
    private readonly BlogContext _context;
    public EditUserRoleClaimsModel(UserManager<appUser> userManager, RoleManager<IdentityRole> roleManager, BlogContext context)
    {
        this.userManager = userManager;
        this.roleManager = roleManager;
        _context = context;
    }

    [BindProperty]
    public InputModel Input { get; set; }

    public class InputModel
    {
        [Required]
        [StringLength(256, MinimumLength = 3)]
        [DisplayName("Claim Type")]
        public string ClaimType { get; set; }

        [Required]
        [StringLength(256, MinimumLength = 3)]
        [DisplayName("Claim Value")]
        public string ClaimValue { get; set; }
    }
    [TempData]
    public string StatusMessage { get; set; }
    public appUser? user { get; set; }
    public async Task<IActionResult> OnGetAsync(string id)
    {
        user = await userManager.FindByIdAsync(id);

        if (user == null)
        {
            return NotFound("Cannot Find User");
        }
        
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(string id)
    {
        user = await userManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound("Cannot Find User");
        }
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var Claims = _context.UserClaims.ToList().Where(u => u.UserId == id);

        if (Claims.Any(c => c.ClaimType == Input.ClaimType && c.ClaimValue == Input.ClaimValue))
        {
            StatusMessage = "Cannot Recreating Claim";
            return Page();
        }

        var result = await userManager.AddClaimAsync(user, new Claim(Input.ClaimType, Input.ClaimValue));
        if (!result.Succeeded)
        {
            foreach(var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return Page();
        }
        return RedirectToPage("./Index");
    }
}