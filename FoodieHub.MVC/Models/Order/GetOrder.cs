namespace FoodieHub.MVC.Models.Order
{
    public class GetOrder
    {
        public int OrderID { get; set; }

        public DateTime OrderedAt { get; set; }
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
    }
}
