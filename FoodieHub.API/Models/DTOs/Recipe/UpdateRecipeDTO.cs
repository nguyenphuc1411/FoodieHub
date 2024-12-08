using System.ComponentModel.DataAnnotations;

namespace FoodieHub.API.Models.DTOs.Recipe
{
    public class UpdateRecipeDTO
    {
        public int RecipeID { get; set; }
        [Required]
        [StringLength(255)]
        public string Title { get; set; } = default!;
        public IFormFile? File { get; set; }
        [Required]
        public TimeOnly PrepTime { get; set; }
        [Required]
        public TimeOnly CookTime { get; set; }
        [Required]
        [Range(1, double.MaxValue, ErrorMessage = "Serves must be greater than 0")]
        public int Serves { get; set; }
        [Required]
        public string Ingredients { get; set; } = default!;
        [Required]
        public string Directions { get; set; } = default!;
        [Required]
        public int CategoryID { get; set; }
        public bool? IsActive { get; set; }
    }
}
