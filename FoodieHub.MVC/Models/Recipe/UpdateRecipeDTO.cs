using System.ComponentModel.DataAnnotations;

namespace FoodieHub.MVC.Models.Recipe
{
    public class UpdateRecipeDTO
    {
        public int RecipeID { get; set; }

        [Required(ErrorMessage = "Title is required.")]
        [StringLength(255, ErrorMessage = "Title cannot be longer than 255 characters.")]
        public string Title { get; set; } = default!;
        public string? Description { get; set; }

        [Required(ErrorMessage = "Cook time is required.")]
        public TimeOnly CookTime { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Serves must be greater than 0.")]
        public int Serves { get; set; }

        public string? ImageURL { get; set; }

        public IFormFile? File { get; set; }

        [Required(ErrorMessage = "Active status is required.")]
        public bool IsActive { get; set; }

        [Required(ErrorMessage = "Category is required.")]
        public int CategoryID { get; set; }
        public List<RecipeStepVM> RecipeSteps { get; set; } = new List<RecipeStepVM>();

        public List<IngredientVM> Ingredients { get; set; } = new List<IngredientVM>();
    }
}
