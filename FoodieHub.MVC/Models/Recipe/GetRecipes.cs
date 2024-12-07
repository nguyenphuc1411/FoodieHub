

namespace FoodieHub.MVC.Models.Recipe
{
    public class GetRecipes
    {
        public int RecipeID { get; set; }
        public string Title { get; set; }
        public string ImageURL { get; set; }
        public string Fullname { get; set; }
        public int? TotalComment { get; set; }
        public int? TotalRating { get; set; }
        public double? RatingAverage { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
        public string CategoryName { get; set; }
    }
}
