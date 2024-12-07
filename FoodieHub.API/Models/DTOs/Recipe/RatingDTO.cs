using System.ComponentModel.DataAnnotations;

namespace FoodieHub.API.Models.DTOs.Recipe
{
    public class RatingDTO
    {
       
        [Required]
        [Range(1,5)]
        public int RatingValue { get; set; }
        [Required]
        public int RecipeID { get; set; }
    }
}
