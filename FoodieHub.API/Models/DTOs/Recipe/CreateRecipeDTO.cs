using System.ComponentModel.DataAnnotations;

namespace FoodieHub.API.Models.DTOs.Recipe
{
    public class CreateRecipeDTO
    {
        [MaxLength(255)]
        public string Title { get; set; } = default!;
        public string? Description { get; set; }
        public IFormFile File { get; set; } 
        public TimeOnly CookTime { get; set; }
        public int Serves { get; set; }
        public bool IsActive { get; set; } = true;
        public int CategoryID { get; set; }

        public List<CreateIngredient> Ingredients { get; set; } = new List<CreateIngredient>();

        public List<CreateRecipeSteps> RecipeSteps { get; set; } = new List<CreateRecipeSteps>();

        public List<int> RelativeProducts { get; set; } = new List<int>();
    }

    public class CreateIngredient
    {
        [MaxLength(255)]
        public string Name { get; set; } = default!;
        [Range(0,float.MaxValue)]

        public float Quantity { get; set; }

        [MaxLength(50)]
        public string Unit { get; set; } = default!;

        public int? ProductID { get; set; }
    }

    public class CreateRecipeSteps
    {
        public int Step { get; set; }

        public IFormFile? ImageStep { get; set; }
        public string Directions { get; set; } = default!;
    }
}
