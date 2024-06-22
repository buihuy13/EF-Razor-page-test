
using EFRazor.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using SQLitePCL;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Humanizer;

public class EditUserClaimModel : PageModel
{
    private readonly SignInManager<appUser> signInManager;
    private readonly UserManager<appUser> userManager;
    private readonly BlogContext context;
    public EditUserClaimModel(SignInManager<appUser> signInManager, UserManager<appUser> userManager, BlogContext context)
    {
        this.signInManager = signInManager;
        this.userManager = userManager;
        this.context = context;
    }

    [TempData]
    public string StatusMessage { get; set; }
    public InputModel Input {  get; set; }
    public class InputModel
    {
        [DisplayName("Current Claim Type")]
        public string currentClaimType { get; set; }
        [DisplayName("Current Claim Value")]
        public string currentClaimValue { get; set; }

        [Required]
        [StringLength(256, MinimumLength = 3)]
        [DisplayName("New Claim Type")]
        public string newClaimType { get; set; }

        [Required]
        [StringLength(256, MinimumLength = 3)]
        [DisplayName("New Claim Value")]
        public string newClaimValue { get; set; }
    }

    public class idData
    {
        public string id { get; set; }
        public int claimid { get; set; }
    }

    public idData data { get; set; }
    public appUser? user { get; set; }
    public async Task<IActionResult> OnGetAsync(int claimid, string id)
    {
        user = await userManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound("Cannot Find User");
        }

        var claim = context.UserClaims.Where(c => c.Id == claimid).FirstOrDefault();

        if (claim == null)
        {
            return NotFound("Cannot Find Claim");
        }

        Input = new InputModel()
        {
            currentClaimType = claim.ClaimType,
            currentClaimValue = claim.ClaimValue
        };

        data = new idData()
        {
            id = id,
            claimid = claimid
        };

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int claimid,string id)
    {
        user = await userManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound("Cannot Find User");
        }

        var claims = context.UserClaims.ToList().Where(c => c.UserId == id);
        if (claims.Any(c => c.ClaimType == Input.newClaimType && c.ClaimValue == Input.newClaimValue))
        {
            StatusMessage = "Cannot Recreate Claim"; 
        }
        var claim = context.UserClaims.Where(c => c.Id == claimid).FirstOrDefault();
        if (claim == null)
        {
            return NotFound("Cannot Find Claim");
        }
        data = new idData()
        {
            id = id,
            claimid = claimid
        };

        claim.ClaimType= Input.newClaimType;
        claim.ClaimValue= Input.newClaimValue;

        await context.SaveChangesAsync();
        return RedirectToPage("./EditUserRoleClaims", new { id = user.Id });
    }
    public async Task<IActionResult> OnPostDeleteAsync(int claimid, string id)
    {
        user = await userManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound("Cannot Find User");
        }

        var claim = context.UserClaims.Where(c => c.Id == claimid).FirstOrDefault();
        if (claim == null)
        {
            return NotFound("Cannot Find Claim");
        }

        var result = await userManager.RemoveClaimAsync(user, new Claim(Input.newClaimType, Input.newClaimValue));
        if (!result.Succeeded)
        {
            foreach(var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return Page();
        }

        StatusMessage = "Delete Success";
        return RedirectToPage("./EditUserRoleClaims", new { id = user.Id });
    }
}