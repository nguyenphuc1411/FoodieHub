using System.ComponentModel.DataAnnotations.Schema;

namespace FoodieHub.API.Data.Entities
{
    public class Rating
    {
        public int RatingValue { get; set; }

        // Foregin Key Property
        public int RecipeID { get; set; }

        [Column(TypeName = "nvarchar(450)")]
        public string UserID { get; set; } = default!;
        // Foreign Key Link
        public Recipe Recipe { get; set; } = default!;

        public ApplicationUser User { get; set; } = default!;
    }
}
