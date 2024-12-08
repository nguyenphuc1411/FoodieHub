using System.ComponentModel.DataAnnotations;

namespace FoodieHub.API.Models.DTOs.Recipe
{
    public class CreateRatingDTO
    {
        [Required(ErrorMessage = "Rating value is required.")]
        [Range(1, 5, ErrorMessage = "Rating value must be between 1 and 5.")]
        public int RatingValue { get; set; }

        [Required(ErrorMessage = "Recipe ID is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Recipe ID must be a positive integer.")]
        public int RecipeID { get; set; }
    }
}
