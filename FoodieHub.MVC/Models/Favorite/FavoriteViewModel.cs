using FoodieHub.MVC.Models.Article;
using FoodieHub.MVC.Models.Recipe;

namespace FoodieHub.MVC.Models.Favorite
{
    public class FavoriteViewModel
    {
        public List<GetArticles> FavoriteArticles { get; set; } = new List<GetArticles>();
        public List<GetRecipes> FavoriteRecipes { get; set; } = new List<GetRecipes>();
    }
}
