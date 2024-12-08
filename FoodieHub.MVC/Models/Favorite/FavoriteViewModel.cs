using FoodieHub.API.Models.DTOs.Article;
using FoodieHub.API.Models.DTOs.Recipe;
using FoodieHub.MVC.Models.Article;
namespace FoodieHub.MVC.Models.Favorite
{
    public class FavoriteViewModel
    {
        public IEnumerable<GetArticleDTO> FavoriteArticles { get; set; } = new List<GetArticleDTO>();
        public IEnumerable<GetRecipeDTO> FavoriteRecipes { get; set; } = new List<GetRecipeDTO>();
    }
}
