namespace FoodieHub.API.Models.DTOs.Review
{
    public class ReviewDTO
    {
        public int ReviewID { get; set; }

        public int RatingValue { get; set; }
        public string ReviewContent { get; set; }

        public DateTime ReviewedAt { get; set; } = DateTime.Now;

        // Foregin Key Property
        public int ProductID { get; set; }
        public string? UserID { get; set; }
    }
}
