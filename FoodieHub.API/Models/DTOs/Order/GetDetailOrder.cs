namespace FoodieHub.API.Models.DTOs.Order
{
    public class GetDetailOrder
    {
        public int OrderID { get; set; }

        public DateTime OrderedAt { get; set; }

        public string Email { get; set; } = default!;
        public string Fullname { get; set; } = default!;

        public decimal TotalAmount { get; set; }

        public string Status { get; set; } = default!;

        public bool PaymentMethod { get; set; }

        public bool PaymentStatus { get; set; } = false;

        public decimal? DiscountOfCoupon { get; set; }

        public decimal? Discount { get; set; }

        public string PhoneNumber { get; set; } = default!;

        public string ShippingAddress { get; set; } = default!;

        public string? Note { get; set; }
        public string? QRCode { get; set; }

        public string UserID { get; set; } = default!;
        public CouponForOrder? Coupon { get; set; }

        public PaymentDTO? Payment { get; set; }
        public OrderCancel? OrderCancel { get; set; }

        // Foreign Key Collections
        public ICollection<ProductForOrder> ProductForOrder { get; set; } = default!;
        public ICollection<OrderStatus> OrderStatues { get; set; } = default!;

    }

    public class CouponForOrder
    {
        public int CouponID { get; set; }
        public string CouponCode { get; set; } = default!;
    }

    public class ProductForOrder
    {
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public int Discount { get; set; }

        public decimal TotalPrice { get; set; }
        public int ProductID { get; set; }
        public string ProductName { get; set; } = default!;
        public string ProductImage { get; set; } = default!;
    }

    public class OrderCancel
    {
        public int Id { get; set; }

        public DateTime CancelledDate { get; set; }

        public string Reason { get; set; } = default!;

        public int OrderID { get; set; }

        public string UserID { get; set; } = default!;
    }

    public class OrderStatus
    {
        public int Id { get; set; }

        public int OrderID { get; set; }

        public string Status { get; set; } = default!;
        public DateTime ChangeDate { get; set; }
    }
    public class PaymentDTO
    {
        public int PaymentID { get; set; }

        public string PaymentMethod { get; set; } = default!;

        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; } = DateTime.Now;

        public int OrderID { get; set; }
    }

}
