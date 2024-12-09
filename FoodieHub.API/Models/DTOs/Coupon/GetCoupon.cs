namespace FoodieHub.API.Models.DTOs.Coupon
{
    public class GetCoupon
    {
        public int CouponID { get; set; }
        public string CouponCode { get; set; }
        public string DiscountType { get; set; }
        public decimal DiscountValue { get; set; }
        public decimal MinimumOrderAmount { get; set; }
        public string? Note { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; } = true;

        public bool IsUsed { get; set; } = false;
    }
}
