namespace FoodieHub.MVC.Models.Product
{
    public class GetOrderDetailsByProductIdDTO
    {
        public int ReviewID { get; set; }

        public int RatingValue { get; set; }
        public string ReviewContent { get; set; }

        public DateTime ReviewedAt { get; set; }

        public int ShelfLife { get; set; }

        // Foregin Key Property
        public string Avatar { get; set; }
        public string UserName { get; set; }
        public int ProductID { get; set; }
        public string Id { get; set; }
    }
}
