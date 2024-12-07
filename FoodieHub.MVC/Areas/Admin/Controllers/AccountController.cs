using FoodieHub.MVC.Configurations;
using FoodieHub.MVC.Models.Request;
using FoodieHub.MVC.Models.Response;
using FoodieHub.MVC.Models.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net.Http.Headers;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace FoodieHub.MVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AccountController : Controller
    {
        private readonly HttpClient _httpClient;
        public AccountController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("MyAPI");
        }
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginVM login)
        {
            if(!ModelState.IsValid) return View(login);

            var httpResponse = await _httpClient.PostAsJsonAsync("auth/admin/login", login);
            var data = await httpResponse.Content.ReadFromJsonAsync<APIResponse>();
            if (data != null)
            {
                if (data.Success)
                {
                    TempData["SuccessMessage"] = data.Message;
                    var cookieOptions = new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true,
                        Expires = DateTime.UtcNow.AddDays(30)
                    };
                    var cookieOptions1 = new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true,
                        Expires = DateTime.UtcNow.AddDays(-1)
                    };
                    Response.Cookies.Append("TokenUser", string.Empty, cookieOptions1);
                    Response.Cookies.Append("Name", string.Empty, cookieOptions1);
                    Response.Cookies.Append("Avatar", string.Empty, cookieOptions1);

                    Response.Cookies.Append("TokenAdmin", data.Data.ToString(), cookieOptions);

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    TempData["ErrorMessage"] = data.Message;
                    return View(login);
                }

            }
            TempData["ErrorMessage"] = "Server Error, please try again!";
            return View(login);
        }

        [ValidateTokenForAdmin]
        public IActionResult Logout()
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                Expires = DateTime.UtcNow.AddDays(-1) 
            };
            Response.Cookies.Append("AvatarAdmin", string.Empty, cookieOptions);
            Response.Cookies.Append("NameAdmin", string.Empty, cookieOptions);
            Response.Cookies.Append("TokenAdmin", string.Empty, cookieOptions);
            return RedirectToAction("Login");
        }
        [ValidateTokenForAdmin]
        public async Task<IActionResult> UpdateProfile()
        {
            var response = await _httpClient.GetAsync("auth/profile");
            var data = await response.Content.ReadFromJsonAsync<APIResponse<UpdateUser>>();
            if (data != null)
            {
                if (data.Success)
                {
                    return View(data.Data);
                }
            }
            else
            {
                TempData["ErrorMessgae"] = "Some thing went wrong. Please try again";
            }
            return RedirectToAction("Profile");
        }
        [ValidateTokenForAdmin]
        [HttpPost]
        public async Task<IActionResult> UpdateProfile(UpdateUser user)
        {
            if (ModelState.IsValid)
            {
                using (var content = new MultipartFormDataContent())
                {
                    content.Add(new StringContent(user.Fullname), "Fullname");

                    if (!string.IsNullOrEmpty(user.Bio))
                    {
                        content.Add(new StringContent(user.Bio), "Bio");
                    }
                    if (!string.IsNullOrEmpty(user.OldPassword) && !string.IsNullOrEmpty(user.NewPassword))
                    {
                        content.Add(new StringContent(user.OldPassword), "OldPassword");
                        content.Add(new StringContent(user.NewPassword), "NewPassword");
                    }
                    if (user.File != null && user.File.Length > 0)
                    {
                        var fileContent = new StreamContent(user.File.OpenReadStream());
                        fileContent.Headers.ContentType = new MediaTypeHeaderValue(user.File.ContentType);
                        content.Add(fileContent, "File", user.File.FileName);
                    }

                    var httpResponse = await _httpClient.PutAsync("auth", content);
                    var data = await httpResponse.Content.ReadFromJsonAsync<APIResponse>();
                    if (data != null)
                    {
                        if (data.Success)
                        {
                            TempData["SuccessMessage"] = data.Message;
                            return RedirectToAction("UpdateProfile");
                        }
                        else
                        {
                            TempData["ErrorMessage"] = data.Message;
                        }
                    }
                }
            }
            return View(user);
        }
    }
}
