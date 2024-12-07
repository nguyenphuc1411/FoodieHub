using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace FoodieHub.API.Data.Entities
{
    public class Coupon
    {
        [Key]
        public int CouponID { get; set; }


        [Column(TypeName = "varchar(20)")]
        public string CouponCode { get; set; } = default!;

        [Column(TypeName = "nvarchar(10)")]
        public string DiscountType { get; set; } = default!;


        [Column(TypeName = "DECIMAL(18,2)")]
        public decimal DiscountValue { get; set; }

        [Column(TypeName = "DECIMAL(18,2)")]
        public decimal MinimumOrderAmount { get; set; }


        [Column(TypeName = "nvarchar(255)")]
        public string? Note { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public bool IsActive { get; set; } = true;

        public bool IsUsed { get; set; } = false;

        public Order? Order { get; set; }
    }
}
