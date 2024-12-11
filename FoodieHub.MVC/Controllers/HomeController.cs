using FoodieHub.MVC.Models.User;
using FoodieHub.MVC.Helpers;
using FoodieHub.MVC.Models.Response;
using FoodieHub.MVC.Service.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FoodieHub.MVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly IAuthService authService;

        public HomeController(IAuthService authService)
        {
            this.authService = authService;
        }

        public async Task<IActionResult> Index()
        {
            var token = Request.GetCookie("TokenUser");
            
            if (!string.IsNullOrEmpty(token))
            {
                var user = await authService.GetProfile();            
                if(user != null)
                {
                    Response.SetCookie("UserID",user.Id);
                    Response.SetCookie("FullName", user.Fullname);
                    if (!string.IsNullOrEmpty(user?.Avatar))
                    {
                        Response.SetCookie("Avatar", user.Avatar);
                    }
                }           
            }
            return View();
        }       
    }
}
