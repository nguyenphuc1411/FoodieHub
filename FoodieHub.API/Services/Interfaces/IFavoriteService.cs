using FoodieHub.API.Data.Entities;
using FoodieHub.API.Models.Response;

namespace FoodieHub.API.Services.Interfaces
{
    public interface IFavoriteService
    {
        Task<Favorite?> Create(Favorite favorite);

        Task<bool> Delete(int id);
       /* Task<ServiceResponse> GetFavoriteArticle();
        Task<ServiceResponse> GetFavoriteRecipe();*/

        Task<ServiceResponse> GetArticleRanking();

    }
}
