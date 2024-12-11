using FoodieHub.MVC.Models.Article;
using FoodieHub.MVC.Models.Recipe;
namespace FoodieHub.MVC.Models.Favorite
{
    public class FavoriteViewModel
    {
        public IEnumerable<GetArticleDTO> FavoriteArticles { get; set; } = new List<GetArticleDTO>();
        public IEnumerable<GetRecipeDTO> FavoriteRecipes { get; set; } = new List<GetRecipeDTO>();
    }
}
