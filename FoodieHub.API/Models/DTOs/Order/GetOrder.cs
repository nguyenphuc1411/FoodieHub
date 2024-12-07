namespace FoodieHub.API.Models.DTOs.Order
{
    public class GetOrder
    {
        public int OrderID { get; set; }

        public DateTime OrderedAt { get; set; }
        public string Fullname { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }
        public bool PaymentMethod { get; set; }
        public bool PaymentStatus { get; set; } = false;
        public decimal? DiscountOfCoupon { get; set; }
        public decimal? Discount { get; set; }
        public string PhoneNumber { get; set; }
        public string ShippingAddress { get; set; }

        public string? Note { get; set; }
        public string? QRCode { get; set; }
        public string UserID { get; set; }
    }
}
