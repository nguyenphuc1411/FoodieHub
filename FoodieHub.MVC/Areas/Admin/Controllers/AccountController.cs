
using FoodieHub.MVC.Models.Authentication;
using FoodieHub.MVC.Models.User;
using FoodieHub.MVC.Configurations;
using Microsoft.AspNetCore.Mvc;
using FoodieHub.MVC.Service.Interfaces;
using FoodieHub.MVC.Helpers;

namespace FoodieHub.MVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AccountController : Controller
    {
        private readonly IAuthService authService;
        public AccountController(IAuthService authService)
        {
            this.authService = authService;
        }
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginDTO login)
        {
            if(!ModelState.IsValid) return View(login);
            var result = await authService.AdminLogin(login);
            if (result.Success)
            {
                NotificationHelper.SetSuccessNotification(this,result.Message);              
                Response.DeleteCookie("TokenUser");
                Response.DeleteCookie("Name");
                Response.DeleteCookie("Avatar");

                Response.SetCookie("TokenAdmin", result.Data.ToString()??"");
                return RedirectToAction("Index", "Home");
            }
            else
            {
                NotificationHelper.SetErrorNotification(this,result.Message);
                return View(login);
            }
        }

        [ValidateTokenForAdmin]
        public IActionResult Logout()
        {
            Response.DeleteCookie("FullNameAdmin");
            Response.DeleteCookie("AvatarAdmin");
            Response.DeleteCookie("TokenAdmin");
            return RedirectToAction("Login");
        }
        [ValidateTokenForAdmin]
        public async Task<IActionResult> UpdateProfile()
        {
            var user = await authService.GetProfile() ?? new UserDTO();
            var update = new UpdateProfileDTO
            {
                Fullname = user.Fullname,
                Bio = user.Bio,
                Avatar = user.Avatar ?? string.Empty,
                Email = user.Email
            };
            return View(update);
        }
        [ValidateTokenForAdmin]
        [HttpPost]
        public async Task<IActionResult> UpdateProfile(UpdateProfileDTO user)
        {
            if (ModelState.IsValid)
            {
                var result = await authService.UpdateProfile(user);
                if (result!=null&& result.Success)
                {
                    NotificationHelper.SetSuccessNotification(this,result.Message);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    NotificationHelper.SetErrorNotification(this, result?.Message);
                }
            }
            return View(user);
        }
    }
}
