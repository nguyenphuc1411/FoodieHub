using FoodieHub.API.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace FoodieHub.API.Configurations.CustomAuthorization
{
    public class AdminRequirementHandler : AuthorizationHandler<AdminRequirement>
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminRequirementHandler(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, AdminRequirement requirement)
        {
            var userID = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userID == null)
            {
                context.Fail();
                return;
            }
            var user = await _userManager.FindByIdAsync(userID);
            if(user == null)
            {
                context.Fail();
                return;
            }
            bool role = await _userManager.IsInRoleAsync(user, "Admin");
            if (role)
            {
                context.Succeed(requirement);
                return;
            }
            context.Fail();
            return ;
        }
    }
}
