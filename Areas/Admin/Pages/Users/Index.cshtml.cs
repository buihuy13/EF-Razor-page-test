using EFRazor.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NuGet.Packaging;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace Users
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<appUser> _userManager;
        [TempData]
        public string StatusMessage { get; set; }
        public IndexModel(UserManager<appUser> userManager)
        {
            _userManager = userManager;
        }
        public class UserAndRoles : appUser
        {
            public string? RoleNames { get; set; }
        }
        public List<UserAndRoles>? Users { get; set; }

        public const int ITEMS_PER_PAGE = 15;

        [BindProperty(SupportsGet = true, Name = "p")]
        public int currentPage { set; get; }
        public int countPages { set; get; }
        public int totalUsers { set; get; }

        public string[,] roleNames;
        public async Task OnGetAsync([FromQuery]string SearchString)
        {
            var UsersList = await _userManager.Users.OrderBy(u => u.UserName).ToListAsync();
            if (UsersList != null)
            {
                totalUsers = UsersList.Count();
                countPages = totalUsers / ITEMS_PER_PAGE;
                if (totalUsers % ITEMS_PER_PAGE != 0)
                {
                    countPages++;
                }
                if (currentPage < 1)
                {
                    currentPage = 1;
                }
                if (currentPage > countPages)
                {
                    currentPage = countPages;
                }
                var users = (from u in UsersList
                             orderby u.UserName descending
                             select u).Skip((currentPage - 1) * ITEMS_PER_PAGE).Take(ITEMS_PER_PAGE)
                                      .Select(u => new UserAndRoles()
                                      {
                                          Id = u.Id,
                                          UserName = u.UserName,
                                      });

                Users = users.ToList();
                foreach(var user in Users)
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    user.RoleNames = string.Join(",", roles);
                }
            }            
        }
    }
}