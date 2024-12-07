namespace FoodieHub.MVC.Models.Order
{
    public class GetOrderDetailsByProductIdDTO
    {
        public int ProductID { get; set; }
        public decimal TotalPrice {  get; set; }

        public int Quantity { get; set; }
        public int OrderID { get; set; }
        
        public string Status { get; set; }
    }
}
