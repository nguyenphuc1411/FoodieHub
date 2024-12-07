using System.ComponentModel.DataAnnotations;

namespace FoodieHub.MVC.Models.Recipe
{
    public class EditRecipe
    {
        public int RecipeID { get; set; }
        [Required]
        [StringLength(255)]
        public string Title { get; set; }
        public IFormFile? File { get; set; }
        [Required]
        [Range(0, 24, ErrorMessage = "PrepHours must be between 0 to 24")]
        public int PrepHours { get; set; }
        [Required]
        [Range(0, 60, ErrorMessage = "PrepMinutes must be between 1 to 60")]
        public int PrepMinutes { get; set; }
        [Required]
        [Range(0, 24, ErrorMessage = "CookHours must be between 0 to 24")]
        public int CookHours { get; set; }
        [Required]
        [Range(0, 60, ErrorMessage = "CookMinutes must be between 1 to 60")]
        public int CookMinutes { get; set; }
        [Required]
        [Range(1, 1000, ErrorMessage = "Serves must be greater than 0")]
        public int Serves { get; set; }
        [Required(ErrorMessage = "Ingredients filed is required")]
        public string Ingredients { get; set; }
        [Required(ErrorMessage = "Directions filed is required")]
        public string Directions { get; set; }
        [Required]
        public int CategoryID { get; set; }
        public bool? IsActive { get; set; }

        public string? ImageURL { get; set; }
    }
}
