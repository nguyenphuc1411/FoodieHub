using System.ComponentModel.DataAnnotations.Schema;

namespace FoodieHub.MVC.Models.Product
{
    public class UpdateReviewDTO
    {
        public int ReviewID { get; set; }

        public int RatingValue { get; set; }


        public string ReviewContent { get; set; }

        public DateTime ReviewedAt { get; set; } = DateTime.Now;

        [NotMapped]
        public int ProductID { get; set; }
    }
}
