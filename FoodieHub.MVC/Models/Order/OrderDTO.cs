using System.ComponentModel.DataAnnotations;

namespace FoodieHub.MVC.Models.Order
{
    public class OrderDTO
    {
        public int? CouponID { get; set; }

        [MaxLength(255)]
        public string ShippingAddress { get; set; } = default!;

        [StringLength(11, MinimumLength = 10)]
        [RegularExpression(@"^(84|0[3|5|7|8|9])([0-9]{8})$", ErrorMessage = "Invalid phone numbber.")]
        public string PhoneNumber { get; set; } = default!;
        public string? Note { get; set; }
        public bool PaymentMethod { get; set; }
        public List<OrderDetailDtO> OrderDetails { get; set; } = new List<OrderDetailDtO>();
    }
    public class OrderDetailDtO
    {
        public int ProductID { get; set; }
        public int Quantity { get; set; }
    }
}
