namespace FoodieHub.MVC.Models.Cart
{
    public class GetCartDTO
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public decimal Discount { get; set; }
        public string MainImage { get; set; }
        public int Quantity { get; set; }
        public int StockQuantity { get; set; }
    }
}
