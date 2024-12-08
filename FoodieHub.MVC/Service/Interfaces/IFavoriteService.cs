using FoodieHub.API.Models.DTOs.Article;
using FoodieHub.API.Models.DTOs.Favorite;
using FoodieHub.API.Models.DTOs.Recipe;

namespace FoodieHub.MVC.Service.Interfaces
{
    public interface IFavoriteService
    {
        Task<bool> Create(CreateFavoriteDTO favorite);
        Task<bool> Delete(int id);

        Task<IEnumerable<GetRecipeDTO>?> GetFR();
        Task<IEnumerable<GetArticleDTO>?> GetFA();
    }
}
