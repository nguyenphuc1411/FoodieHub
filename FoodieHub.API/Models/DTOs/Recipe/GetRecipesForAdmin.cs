namespace FoodieHub.API.Models.DTOs.Recipe
{
    public class GetRecipesForAdmin
    {
        public int RecipeID { get; set; }
        public string Title { get; set; }
        public string ImageURL { get; set; }
        public string Fullname { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
        public int CategoryID { get; set; }
        public string CategoryName { get; set; }
        public bool IsActive { get; set; }
    }
}
