using FoodieHub.MVC.Configurations;
using FoodieHub.MVC.Helpers;
using FoodieHub.MVC.Models.Coupon;
using FoodieHub.MVC.Models.Response;
using FoodieHub.MVC.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace FoodieHub.MVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    [ValidateTokenForAdmin]
    public class CouponsController : Controller
    {
        private readonly IConfiguration _config;
        private readonly ICouponService couponService;
        public CouponsController(IConfiguration config, ICouponService couponService)
        {
            _config = config;
            this.couponService = couponService;
        }

        // Phương thức hiển thị danh sách coupon
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var coupons = await couponService.Get();
            return View(coupons);
        }

        private string GenerateDefaultCouponCode()
        {
            // Generate a default coupon code in the format "COUPON-XXXXXX"
            return "COUPON-" + Guid.NewGuid().ToString().Substring(0, 8).ToUpper();
        }

        public IActionResult CreateCoupon()
        {
            var coupon = new CouponDTO
            {
                CouponCode = GenerateDefaultCouponCode() // Generate the coupon code here
            };
            return View(coupon);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCoupon(CouponDTO coupon)
        {
            if (!ModelState.IsValid) return View(coupon);
            var result = await couponService.Create(coupon);
            if (result)
            {
                NotificationHelper.SetSuccessNotification(this);
                return RedirectToAction("Index");
            }
            NotificationHelper.SetErrorNotification(this);
            return View(coupon);
        }

        // GET: Admin/Coupons/EditCoupon/5
        public async Task<IActionResult> EditCoupon(int id)
        {
            var client = _httpClientFactory.CreateClient("MyAPI");

            var response = await client.GetAsync($"Coupons/{id}");
            var content = await response.Content.ReadFromJsonAsync<APIResponse<CouponDTO>>();

            if (content.Success)
            {
                return View(content.Data);
            }

            TempData["ErrorMessage"] = content.Message;
            return RedirectToAction("Index");
        }

        // POST: Admin/Coupons/EditCoupon
        [HttpPost]
        public async Task<IActionResult> EditCoupon(int id, CouponDTO coupon)
        {
            if (!ModelState.IsValid)
            {
                return View(coupon);
            }

            var client = _httpClientFactory.CreateClient("MyAPI");

            var jsonContent = new StringContent(JsonSerializer.Serialize(coupon), Encoding.UTF8, "application/json");

            var response = await client.PutAsync($"Coupons/{id}", jsonContent);

            var data = await response.Content.ReadFromJsonAsync<APIResponse>();
            if (data != null)
            {
                if (data.Success)
                {
                    TempData["SuccessMessage"] = data.Message;
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["ErrorMessage"] = data.Message;
                }
            }
            else
            {
                TempData["ErrorMessage"] = "An unexpected error occurred. Please try again.";
            }

            return View(coupon);
        }

        public async Task<IActionResult> DeleteCoupon(int id)
        {
            var client = _httpClientFactory.CreateClient("MyAPI");

            var response = await client.DeleteAsync($"Coupons/{id}");
            var data = await response.Content.ReadFromJsonAsync<APIResponse>();

            if (data.Success)
            {
                TempData["SuccessMessage"] = data.Message;
            }
            else
            {
                TempData["ErrorMessage"] = data.Message;
            }
            return RedirectToAction("Index");
        }
    }
}
