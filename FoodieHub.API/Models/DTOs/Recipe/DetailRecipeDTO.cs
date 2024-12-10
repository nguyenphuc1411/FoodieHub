
namespace FoodieHub.API.Models.DTOs.Recipe
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

        public IEnumerable<GetIngredient> Ingredients { get; set; }= new List<GetIngredient>();
        public IEnumerable<GetRecipeStep> Steps { get; set; }= new List<GetRecipeStep>();
        public List<int> RelativeProducts { get; set; } = new List<int>();
    }


    public class GetIngredient
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;
        public float Quantity { get; set; }
        public string Unit { get; set; } = default!;
        public int? ProductID { get; set; }
    }
    public class GetRecipeStep
    {
        public int Id { get; set; }
        public int Step { get; set; }
        public string? ImageURL { get; set; }
        public string Directions { get; set; } = default!;

    }
}
