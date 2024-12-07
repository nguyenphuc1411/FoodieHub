using Microsoft.AspNetCore.Authorization;

namespace FoodieHub.API.Configurations.CustomAuthorization
{
    public class AdminRequirement:IAuthorizationRequirement
    {
        public AdminRequirement()
        {
            
        }
    }
}
