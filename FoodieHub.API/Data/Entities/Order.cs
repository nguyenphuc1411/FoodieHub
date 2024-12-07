using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace FoodieHub.API.Data.Entities
{
    public class Order
    {
        [Key]
        public int OrderID { get; set; }

        public DateTime OrderedAt { get; set; } = DateTime.Now;

        [Column(TypeName = "DECIMAL(18,2)")]
        public decimal TotalAmount { get; set; }

        [Column(TypeName = "nvarchar(50)")]
        public string Status { get; set; } = default!;

        public bool PaymentMethod { get; set; }

        public bool PaymentStatus { get; set; } = false;

        [Column(TypeName = "DECIMAL(18,2)")]
        public decimal? DiscountOfCoupon { get; set; }

        [Column(TypeName = "DECIMAL(18,2)")]
        public decimal? Discount { get; set; }

        [Column(TypeName = "nvarchar(11)")]
        public string PhoneNumber {  get; set; } = default!;


        [Column(TypeName = "nvarchar(255)")]
        public string ShippingAddress { get; set; } = default!;


        [Column(TypeName = "nvarchar(255)")]
        public string? Note { get; set; }

        public string? QRCode { get; set; }

        [Column(TypeName = "nvarchar(450)")]
        public string UserID { get; set; } = default!;
        public ApplicationUser User { get; set; } = default!;

        public int? CouponID { get; set; }
        public Coupon? Coupon { get; set; }
        
        public Payment? Payment { get; set; }
        public OrderCancellation? OrderCancellation { get; set; }

        // Foreign Key Collections
        public ICollection<OrderDetail> OrderDetails { get; set; } = default!;
        public ICollection<OrderStatusHistory> OrderStatusHistories { get; set; } = default!;

    }
}
