namespace FoodieHub.MVC.Areas.Admin.Models
{
    public class TopSelling
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public string ImageUrl { get; set; }
        public decimal Price { get; set; }
        public int Sold { get; set; }
        public decimal TotalRevenue { get; set; }
    }
}
