namespace FoodieHub.API.Models.DTOs.Order
{
    public class RecentlyOrder
    {
        public int OrderID { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }
        public string PhoneNumber { get; set; }
        public string FullName { get; set; }
    }
}
