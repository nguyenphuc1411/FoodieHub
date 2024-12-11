using FoodieHub.MVC.Models.Article;
using FoodieHub.MVC.Models.Favorite;
using FoodieHub.MVC.Models.Recipe;

namespace FoodieHub.MVC.Service.Interfaces
{
    public interface IFavoriteService
    {
        Task<bool> Create(FavoriteDTO favorite);
        Task<bool> Delete(FavoriteDTO favorite);

        Task<IEnumerable<GetRecipeDTO>?> GetFR();
        Task<IEnumerable<GetArticleDTO>?> GetFA();
    }
}
