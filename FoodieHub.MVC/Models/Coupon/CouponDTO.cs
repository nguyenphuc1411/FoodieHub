using FoodieHub.MVC.Areas.Admin.Models;
using System.ComponentModel.DataAnnotations;

namespace FoodieHub.MVC.Models.Coupon
{
    public class CouponDTO
    {
        [StringLength(20)]
        public string CouponCode { get; set; }
        [StringLength(10)]
        public string DiscountType { get; set; }

        [DiscountValidation]
        public decimal DiscountValue { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "The value must be a positive number.")]
        public decimal MinimumOrderAmount { get; set; }

        [StringLength(255)]
        public string? Note { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }
    }
}
