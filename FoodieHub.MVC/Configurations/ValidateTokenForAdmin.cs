using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace FoodieHub.MVC.Configurations
{
    public class ValidateTokenForAdmin:ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var token = context.HttpContext.Request.Cookies["TokenAdmin"];
            if (string.IsNullOrEmpty(token))
            {
                context.Result = new RedirectToRouteResult(new RouteValueDictionary
                {
                    { "area", "Admin" },
                    { "controller", "Account" },
                    { "action", "Login" }
                });
                return;
            }
            base.OnActionExecuting(context);
        }
    }
}
