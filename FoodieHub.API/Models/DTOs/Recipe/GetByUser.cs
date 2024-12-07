namespace FoodieHub.API.Models.DTOs.Recipe
{
    public class GetByUser
    {
        public int RecipeID { get; set; }
        public string Title { get; set; }
        public string ImageURL { get; set; }
        public bool IsActive { get; set; }
        public int? TotalComment { get; set; }
        public int? TotalRating { get; set; }
        public double? RatingAverage { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
        public int CategoryID { get; set; }
        public string CategoryName { get; set; }
    }
}
