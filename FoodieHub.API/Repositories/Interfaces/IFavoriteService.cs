using FoodieHub.API.Data.Entities;
using FoodieHub.API.Models.DTOs.Article;
using FoodieHub.API.Models.DTOs.Favorite;
using FoodieHub.API.Models.DTOs.Recipe;
using FoodieHub.API.Models.Response;

namespace FoodieHub.API.Repositories.Interfaces
{
    public interface IFavoriteService
    {
        Task<Favorite?> Create(CreateFavoriteDTO favorite);

        Task<bool> Delete(int id);
        Task<IEnumerable<GetArticleDTO>> GetFavoriteArticle();
        Task<IEnumerable<GetRecipeDTO>> GetFavoriteRecipe();

    }
}
