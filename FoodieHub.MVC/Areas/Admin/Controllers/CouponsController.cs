using FoodieHub.MVC.Configurations;
using FoodieHub.MVC.Helpers;
using FoodieHub.MVC.Models.Coupon;
using FoodieHub.MVC.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FoodieHub.MVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    [ValidateTokenForAdmin]
    public class CouponsController : Controller
    {
        private readonly ICouponService couponService;
        public CouponsController(ICouponService couponService)
        {
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
            var result = await couponService.GetDetail(id);
            if (result == null)
            {
                NotificationHelper.SetErrorNotification(this);
                return RedirectToAction("Index");
            }

            var editCoupon = new CouponDTO
            {
                CouponCode =result.CouponCode,
                DiscountType = result.DiscountType,
                DiscountValue = result.DiscountValue,
                MinimumOrderAmount = result.MinimumOrderAmount,
                StartDate = result.StartDate,
                EndDate = result.EndDate,
                Note = result.Note,
                IsActive = result.IsActive,
            };
            return View(editCoupon);
        }

        // POST: Admin/Coupons/EditCoupon
        [HttpPost]
        public async Task<IActionResult> EditCoupon(int id, CouponDTO coupon)
        {
            if (!ModelState.IsValid)
            {
                return View(coupon);
            }
            var result = await couponService.Update(id, coupon);

            if (result)
            {
                NotificationHelper.SetSuccessNotification(this);
                return RedirectToAction("Index");
            }

            else NotificationHelper.SetErrorNotification(this);
            return View(coupon);
        }

        public async Task<IActionResult> DeleteCoupon(int id)
        {
            bool result = await couponService.Delete(id);

            if (result) NotificationHelper.SetSuccessNotification(this);
            else NotificationHelper.SetErrorNotification(this);
            return RedirectToAction("Index");
        }
    }
}
