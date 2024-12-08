namespace FoodieHub.API.Models.DTOs.Favorite
{
    public class CreateFavoriteDTO
    {
        public int? ArticleID { get; set; }
        public int? RecipeID { get; set; }
    }
}
