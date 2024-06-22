﻿using EFRazor.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Dynamic;
using System.Runtime.CompilerServices;

namespace Roles
{
    [Authorize(Policy = "AllowEditRole")]
    public class EditModel : RolePageModel
    {
        public EditModel(RoleManager<IdentityRole> roleManager, BlogContext context) : base(roleManager, context)
        {
        }

        public List<IdentityRoleClaim<string>> Claims { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            [StringLength(256)]
            [DisplayName("New Name")]
            public string newName { get; set; }

            [StringLength(256)]
            [DisplayName("Current Name")]
            public string? currentName { get; set; }
        }
        public IdentityRole? role { get; set; }
        public async Task<IActionResult> OnGet(string roleid)
        {
            role = await _RoleManager.FindByIdAsync(roleid);

            if (role == null)
            {
                StatusMessage = "Cannot find role";
                return RedirectToPage("./Index");
            }

            Input = new InputModel()
            {
                currentName = role.Name
            };
            Claims = await _context.RoleClaims.Where(c => c.RoleId == role.Id).ToListAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string roleid)
        {
            var role = await _RoleManager.FindByIdAsync(roleid);
            if (!ModelState.IsValid)
            {
                return Page();
            }
            if (role != null)
            {
                role.Name = Input.newName;
                var result = await _RoleManager.UpdateAsync(role);
                if (!result.Succeeded)
                {
                    foreach(var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                        return Page();
                    }
                }
                StatusMessage = "Success Edit";
                return RedirectToPage("./Index");
            }
            return Page();
        }
    }
}