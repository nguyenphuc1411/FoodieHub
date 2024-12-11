using FoodieHub.API.Models.DTOs.Article;
using FoodieHub.API.Models.DTOs.Favorite;
using FoodieHub.API.Models.DTOs.Recipe;

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
