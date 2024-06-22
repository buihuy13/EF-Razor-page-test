
using EFRazor.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Security.Requirements
{
    public class AppAuthorizationHandler : IAuthorizationHandler
    {
        private readonly ILogger<AppAuthorizationHandler> _logger;

        private readonly UserManager<appUser> userManager;

        public AppAuthorizationHandler(ILogger<AppAuthorizationHandler> logger, UserManager<appUser> userManager)
        {
            _logger = logger;
            this.userManager = userManager;
        }

        public Task HandleAsync(AuthorizationHandlerContext context)
        {
            var requirements = context.PendingRequirements.ToList();

            foreach (var requirement in requirements)
            {
                if (requirement is GenZRequirement)
                {
                    if (IsGenZ(context.User, (GenZRequirement)requirement)) //Nhận về các thông tin của user và các requirement đối với GenZRequirement
                    {
                        context.Succeed(requirement);
                    }
                }

                /*
                  if (requirement is OtherRequirement)
                  {
                    ...
                  }
                 */
            }
            
            return Task.CompletedTask;
        }

        private bool IsGenZ(ClaimsPrincipal user, GenZRequirement requirement)
        {
            var appUser = userManager.GetUserAsync(user);
            Task.WaitAll(appUser);
            var User = appUser.Result;

            if (User == null || User.DateOfBirth == null)
            {
                return false;
            }
            var year = User.DateOfBirth.Value.Year;

            return (year >= requirement.FromYear && year <= requirement.ToYear);
            throw new NotImplementedException();
        }
    }
}
