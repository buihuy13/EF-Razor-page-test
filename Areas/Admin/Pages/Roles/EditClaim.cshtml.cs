using EFRazor.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Roles;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

public class EditClaimModel : RolePageModel
{
    public EditClaimModel(RoleManager<IdentityRole> roleManager, BlogContext context) : base(roleManager, context)
    {
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
    public IdentityRole? role { get; set; }

    public int? claimId {  get; set; } 
    public async Task<IActionResult> OnGetAsync(int claimid)
    {
        var claim = await _context.RoleClaims.Where(c => c.Id == claimid).FirstOrDefaultAsync();

        claimId = claimid;
        if (claim == null)
        {
            return NotFound("Cannot Find Claim");
        }

        role = await _RoleManager.FindByIdAsync(claim.RoleId);

        if (role == null)
        {
            return NotFound("Cannot Find Role With Claim");
        }

        Input = new InputModel()
        {
            ClaimType = claim.ClaimType,
            ClaimValue = claim.ClaimValue
        };
        return Page();
    }
    public async Task<IActionResult> OnPostAsync(int claimid)
    {
        claimId = claimid;
        var claim = await _context.RoleClaims.Where(c => c.Id == claimid).FirstOrDefaultAsync();
        if (claim == null)
        {
            return NotFound("Cannot Find Claim");
        }
        role = await _RoleManager.FindByIdAsync(claim.RoleId);

        if (role == null)
        {
            return NotFound("Cannot Find Role With Claim");
        }

        if (!ModelState.IsValid)
        {
            return Page();
        }

        if (_context.RoleClaims.Any(c => c.ClaimType == Input.ClaimType && c.ClaimValue == Input.ClaimValue))
        {
            StatusMessage = "Error, Cannot Recreate Claim";
            return Page();
        }

        claim.ClaimType = Input.ClaimType;
        claim.ClaimValue = Input.ClaimValue;

        await _context.SaveChangesAsync();
        StatusMessage = "Saved Changes";
        return RedirectToPage("./Edit", new { roleid = role.Id });
    }
    public async Task<IActionResult> OnPostDeleteAsync(int claimid)
    {
        var claim = await _context.RoleClaims.Where(c => c.Id == claimid).FirstOrDefaultAsync();
        if (claim == null)
        {
            return NotFound("Cannot Find Claim");
        }
        role = await _RoleManager.FindByIdAsync(claim.RoleId);

        if (role == null)
        {
            return NotFound("Cannot Find Role With Claim");
        }
        var result = await _RoleManager.RemoveClaimAsync(role, new Claim(claim.ClaimType,claim.ClaimValue));

        if (!result.Succeeded)
        {
            foreach (var error in  result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return Page();
        }

        StatusMessage = "Delete Success";

        return RedirectToPage("./Edit", new { roleid = role.Id });
    }
}

