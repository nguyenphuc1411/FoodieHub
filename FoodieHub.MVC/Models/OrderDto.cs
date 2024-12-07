using System.ComponentModel.DataAnnotations;

namespace FoodieHub.MVC.Models
{
    public class OrderDto
    {
        [Required(ErrorMessage = "Order ID is required")]
        public int OrderID { get; set; }

        [Required(ErrorMessage = "User ID is required")]
        public string UserID { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Total amount must be greater than 0")]
        public decimal TotalAmount { get; set; }

        [Required(ErrorMessage = "Order date is required")]
        public DateTime OrderDate { get; set; }

        [Required(ErrorMessage = "Status is required")]
        [StringLength(20, ErrorMessage = "Status cannot be longer than 20 characters")]
        public string Status { get; set; }

        [Required(ErrorMessage = "Shipping address is required")]
        [StringLength(255, ErrorMessage = "Shipping address cannot be longer than 255 characters")]
        public string ShippingAddress { get; set; }

        public DateTime? ShippingDate { get; set; }

        [StringLength(500, ErrorMessage = "Note cannot be longer than 500 characters")]
        public string? Note { get; set; }

        [Required(ErrorMessage = "Ordered at date is required")]
        public DateTime OrderedAt { get; set; }

        public string? PaymentID { get; set; }

        public string? CouponID { get; set; }

        // Validation cho OrderDetails nếu cần
        public List<OrderDetailDto>? OrderDetails { get; set; }
    }
}
