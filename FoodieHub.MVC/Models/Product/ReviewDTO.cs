using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace FoodieHub.MVC.Models.Product
{
    public class ReviewDTO
    {
        public int ReviewID { get; set; }

        public int RatingValue { get; set; }


        public string ReviewContent { get; set; }

      

        public DateTime ReviewedAt { get; set; } = DateTime.Now;

        // Foregin Key Property
        public int ProductID { get; set; }

    }
}
