
using System.ComponentModel.DataAnnotations;

namespace FoodieHub.MVC.Models.Recipe
{
    public class DetailRecipeDTO
    {
        public int RecipeID { get; set; }
        public string Title { get; set; } = default!;
        public string? Description { get; set; }
        public string ImageURL { get; set; } = default!;
        public TimeOnly CookTime { get; set; }
        public int Serves { get; set; }
        public bool IsAdminUpload { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; }

        public string UserID { get; set; } = default!;
        public string FullName { get; set; } = default!;
        public string? Avatar { get; set; }
        public int CategoryID { get; set; }
        public string CategoryName { get; set; } = default!;

        public int TotalRatings { get; set; } = 0;
        public double RatingAverage { get; set; } = 0;
        public int TotalFavorites { get; set; } = 0;
        public int TotalComments { get; set; } = 0;

        public List<IngredientVM> Ingredients { get; set; }= new List<IngredientVM>();
        public List<RecipeStepVM> Steps { get; set; }= new List<RecipeStepVM>();
        public List<int> RelativeProducts { get; set; } = new List<int>();
    }


    public class IngredientVM
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Ingredient name is required.")]
        [StringLength(100, ErrorMessage = "Ingredient name cannot be longer than 100 characters.")]
        public string Name { get; set; } = default!;

        [Range(0.1, float.MaxValue, ErrorMessage = "Quantity must be greater than 0.")]
        public float Quantity { get; set; }

        [Required(ErrorMessage = "Unit is required.")]
        [StringLength(50, ErrorMessage = "Unit cannot be longer than 50 characters.")]
        public string Unit { get; set; } = default!;

        public int? ProductID { get; set; }
    }
    public class RecipeStepVM
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Step number is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Step number must be greater than 0.")]
        public int Step { get; set; }

        public string? ImageURL { get; set; }

        [DataType(DataType.Upload, ErrorMessage = "Please select a valid image file.")]
        public IFormFile? FileStep { get; set; }

        [Required(ErrorMessage = "Directions are required.")]
        public string Directions { get; set; } = default!;
    }
}
