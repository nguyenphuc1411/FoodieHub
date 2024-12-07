namespace FoodieHub.MVC.Models.Order
{
    public class GetOrderByUserIdDTO
    {
        public int OrderID { get; set; }
        public string UserID { get; set; }  
        public string Status { get; set; }

        public DateTime OrderedAt { get; set; }
    }
}
