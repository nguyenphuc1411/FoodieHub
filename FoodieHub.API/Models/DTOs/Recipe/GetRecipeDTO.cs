namespace FoodieHub.API.Models.DTOs.Recipe
{
    public class GetRecipeDTO
    {
        public int RecipeID { get; set; }
        public string Title { get; set; } = default!;
        public string? Description { get; set; }
        public string ImageURL { get; set; } = default!;
        public TimeOnly CookTime { get; set; }
        public int Serves { get; set; }
        public bool IsAdminUpload { get; set; } 
        public DateTime CreatedAt { get; set; } 
        public bool IsActive { get; set; } = true;

        public string UserID { get; set; } = default!;
        public string FullName { get; set; } = default!;
        public string? Avatar { get; set; }
        public int CategoryID { get; set; }
        public string CategoryName { get; set; } = default!;

        public int TotalRatings { get; set; } = 0;
        public double RatingAverage { get; set; } = 0;
        public int TotalFavorites { get; set; } = 0;
        public int TotalComments { get; set; } = 0;
    }
}
