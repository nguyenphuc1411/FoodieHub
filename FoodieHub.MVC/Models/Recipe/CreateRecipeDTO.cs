using System.ComponentModel.DataAnnotations;

namespace FoodieHub.MVC.Models.Recipe
{
    public class CreateRecipeDTO
    {
        [Required(ErrorMessage = "Title is required.")]
        [MaxLength(255, ErrorMessage = "Title cannot exceed 255 characters.")]
        public string Title { get; set; } = default!;

        public string? Description { get; set; }

        [Required(ErrorMessage = "File is required.")]
        public IFormFile File { get; set; }

        [Required(ErrorMessage = "CookTime is required.")]
        public TimeOnly CookTime { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Serves must be greater than 0.")]
        public int Serves { get; set; }

        public bool IsActive { get; set; } = true;

        [Required(ErrorMessage = "CategoryID is required.")]
        public int CategoryID { get; set; }

        [MinLength(1, ErrorMessage = "At least one ingredient is required.")]
        public List<CreateIngredient> Ingredients { get; set; } = new List<CreateIngredient>();

        [MinLength(1, ErrorMessage = "At least one recipe step is required.")]
        public List<CreateRecipeSteps> RecipeSteps { get; set; } = new List<CreateRecipeSteps>();

        public List<int> RelativeProducts { get; set; } = new List<int>();
    }

    public class CreateIngredient
    {
        [Required(ErrorMessage = "Ingredient name is required.")]
        [MaxLength(255, ErrorMessage = "Ingredient name cannot exceed 255 characters.")]
        public string Name { get; set; } = default!;

        [Range(0.01, float.MaxValue, ErrorMessage = "Quantity must be greater than 0.")]
        public float Quantity { get; set; }

        [Required(ErrorMessage = "Unit is required.")]
        [MaxLength(50, ErrorMessage = "Unit cannot exceed 50 characters.")]
        public string Unit { get; set; } = default!;

        public int? ProductID { get; set; }
    }


    public class CreateRecipeSteps
    {
        [Range(1, int.MaxValue, ErrorMessage = "Step must be a positive integer.")]
        public int Step { get; set; }

        public IFormFile? ImageStep { get; set; }

        [Required(ErrorMessage = "Directions are required.")]
        public string Directions { get; set; } = default!;
    }

}
