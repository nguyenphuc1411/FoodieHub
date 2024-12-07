using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FoodieHub.API.Models.DTOs.Coupon
{
    public class CouponDTO
    {
        [StringLength(20)]
        public string CouponCode { get; set; }
        [StringLength(10)]
        public string DiscountType { get; set; }
        [Range(1, double.MaxValue)]
        public decimal DiscountValue { get; set; }

        [Range(1,double.MaxValue)]
        public decimal MinimumOrderAmount { get; set; }

        [StringLength(255)]
        public string? Note { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }
    }
}
