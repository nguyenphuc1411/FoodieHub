using FoodieHub.MVC.Models;
using FoodieHub.MVC.Models.Response;
using FoodieHub.MVC.Models.User;
using FoodieHub.MVC.Models.VnPay;
using FoodieHub.MVC.Service.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace FoodieHub.MVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly HttpClient _httpClient;

        public HomeController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("MyAPI");
        }

        public async Task<IActionResult> Index()
        {
            var token = Request.Cookies["TokenUser"];
            
            if (!string.IsNullOrEmpty(token))
            {
                var response = await _httpClient.GetAsync("auth/profile");
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadFromJsonAsync<APIResponse<UserDTO>>();
                    if (data != null)
                    {
                        var opt = new CookieOptions
                        {
                            HttpOnly = false,
                            Expires = DateTime.Now.AddDays(30)
                        };
                        Response.Cookies.Append("Name", data.Data.Fullname.ToString(), opt);
                        if (!string.IsNullOrEmpty(data.Data.Avatar))
                        {
                            Response.Cookies.Append("Avatar", data.Data.Avatar.ToString(), opt);
                        }
                    }
                }
            }
            return View();
        }  
       
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public async Task<IActionResult> GetQRCode()
        {
            // Gửi yêu cầu đến API để lấy chuỗi base64
            var response = await _httpClient.GetAsync("qrcode");
            var data = await response.Content.ReadAsStringAsync();

            // Tạo chuỗi base64 cho mã QR
            string img = $"data:image/png;base64,{data}";

            // Trả chuỗi base64 vào view
            return View("GetQRCode", img);
        }

    }
}
