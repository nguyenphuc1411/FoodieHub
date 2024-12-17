using System.ComponentModel.DataAnnotations;

namespace FoodieHub.MVC.Models.Recipe
{
    public class CreateRecipeDTO
    {
        public CreateRecipeDTO()
        {
            Ingredients = new List<CreateIngredient> {
                new() {
                    Name = "Ingredient 1",
                    Quantity = 100,
                    Unit = "Gam (g)"
                }
            };
            RecipeSteps = new List<CreateRecipeSteps>
            {
                new()
                {
                    Step = 1,
                    Directions = "Directions"
                }
            };
        }
        [Required(ErrorMessage = "Title is required.")]
        [MaxLength(255, ErrorMessage = "Title cannot exceed 255 characters.")]
        public string Title { get; set; } = default!;

        public string? Description { get; set; }

        [Required(ErrorMessage = "File is required.")]
        public IFormFile File { get; set; } = default!;

        [Required(ErrorMessage = "Cook time is required.")]
        public TimeOnly CookTime { get; set; }

        [Required(ErrorMessage = "Serves is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Serves must be greater than 0.")]
        public int Serves { get; set; }

        public bool IsActive { get; set; } = true;

        [Required(ErrorMessage = "CategoryID is required.")]
        public int CategoryID { get; set; }
        public List<CreateIngredient> Ingredients { get; set; } = new List<CreateIngredient>();

        public List<CreateRecipeSteps> RecipeSteps { get; set; } = new List<CreateRecipeSteps>();

        public List<int> RelativeProducts { get; set; } = new List<int>();
    }

    public class CreateIngredient
    {
        [Required(ErrorMessage = "Ingredient name is required.")]
        [MaxLength(255, ErrorMessage = "Name cannot exceed 255 characters.")]
        public string Name { get; set; } = default!;

        [Required(ErrorMessage = "Quantity is required.")]
        [Range(0.1, float.MaxValue, ErrorMessage = "Quantity must be a positive number.")]
        public float Quantity { get; set; }

        [Required(ErrorMessage = "Unit is required.")]
        [MaxLength(50, ErrorMessage = "Unit cannot exceed 50 characters.")]
        public string Unit { get; set; } = default!;

        public int? ProductID { get; set; }
    }

    public class CreateRecipeSteps
    {
        [Required(ErrorMessage = "Step number is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Step must be greater than 0.")]
        public int Step { get; set; }

        public IFormFile? ImageStep { get; set; }

        [Required(ErrorMessage = "Directions are required.")]
        public string Directions { get; set; } = default!;
    }
}
