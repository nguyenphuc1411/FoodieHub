namespace FoodieHub.MVC.Models.Recipe
{
    public class GetRecipeDetail
    {
        public int RecipeID { get; set; }

        public string Title { get; set; }

        public string ImageURL { get; set; }

        public TimeOnly PrepTime { get; set; }

        public TimeOnly CookTime { get; set; }

        public int Serves { get; set; }
        public string Ingredients { get; set; }
        public string Directions { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
        public string Fullname { get; set; }
        public string Avatar { get; set; }
        public string CategoryName { get; set; }

        public double? Rating { get; set; }
        public int? TotalRating { get; set; }
        public int? TotalComment { get; set; }
    }
}
