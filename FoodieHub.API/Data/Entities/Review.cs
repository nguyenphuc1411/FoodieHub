using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace FoodieHub.API.Data.Entities
{
    public class Review
    {
        [Key]
        public int ReviewID { get; set; }

        public int RatingValue { get; set; }


        [Column(TypeName = "nvarchar(255)")]
        public string ReviewContent { get; set; } = default!;

        public DateTime ReviewedAt { get; set; } = DateTime.Now;

        // Foregin Key Property
        public int ProductID { get; set; }

        [Column(TypeName = "nvarchar(450)")]
        public string UserID { get; set; } = default!;
        // Foreign Key Link
        public Product Product { get; set; } = default!;

        public ApplicationUser User { get; set; } = default!;
    }
}
