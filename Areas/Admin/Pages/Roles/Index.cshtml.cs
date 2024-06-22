
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

        public class RoleModel : IdentityRole
        {
            public string[] Claims { get; set; }
        }
        public List<RoleModel>? roles { get; set; }
        public async Task OnGet()
        {
            var r = await _RoleManager.Roles.ToListAsync();
            roles = new List<RoleModel>();

            foreach(var role in r)
            {
                var claims = await _RoleManager.GetClaimsAsync(role);
                //Trả về Type của claim : giá trị của claim đó
                var claimString = claims.Select(c => c.Type + " : " + c.Value);
                roles.Add(new RoleModel
                {
                    Name = role.Name,
                    Id = role.Id,
                    Claims = claimString.ToArray()
                });
            }
        }
    }
}